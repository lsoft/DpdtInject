using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DpdtInject.Extension.CodeLens;
using DpdtInject.Extension.Container.Component;
using DpdtInject.Extension.ExtensionStatus.FullyLoaded;
using DpdtInject.Generator;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Injector.Compilation;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.OperationProgress;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TaskStatusCenter;
using Microsoft.VisualStudio.Threading;
using System.Diagnostics;

using Task = System.Threading.Tasks.Task;
using Project = Microsoft.CodeAnalysis.Project;
using Thread = System.Threading.Thread;

using static DpdtInject.Extension.Shared.Logging;

namespace DpdtInject.Extension.Container
{
    [Export(typeof(ContainerAndScanner))]
    public class ContainerAndScanner
    {
        private readonly object _locker = new object();

        private IVsOutputWindowPane? _outputPane;

        private TaskProgressData _data;
        private BackgroundScanner? _backgroundScanner;

        public SolutionBindContainer? Binds => _backgroundScanner?.SolutionBindContainer;

        [ImportingConstructor]
        public ContainerAndScanner(
            )
        {
        }

        public async Task InitializeAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var paneGuid = Guid.NewGuid();
            var outputWindow = (Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow)!;
            outputWindow.CreatePane(ref paneGuid, "Dpdt Extension output", 1, 1);
            outputWindow.GetPane(ref paneGuid, out var pane);
            _outputPane = pane;
            _outputPane.Activate();
            _outputPane.OutputStringThreadSafe($"Dpdt scanner is on{Environment.NewLine}");
        }

        public void AsyncStartScan()
        {
            lock (_locker)
            {
                SyncStopScanInternal();

                AsyncStartScanInternal();
            }
        }

        public void StopScanSync()
        {
            lock (_locker)
            {
                SyncStopScanInternal();
            }
        }

        private void SyncStopScanInternal()
        {
            //stop scanning if it is working now
            _backgroundScanner?.SyncStop();
        }

        private void AsyncStartScanInternal()
        {
            try
            {
                var taskStatusCenterService = AsyncPackage.GetGlobalService(typeof(SVsTaskStatusCenterService)) as IVsTaskStatusCenterService;
                if (taskStatusCenterService == null)
                {
                    return;
                }

                var options = default(TaskHandlerOptions);
                options.Title = "Scanning solution for Dpdt clusters and its bindings...";
                options.ActionsAfterCompletion = CompletionActions.None;
                
                _data = default(TaskProgressData);
                _data.CanBeCanceled = false;

                var handler = taskStatusCenterService.PreRegister(options, _data);


                _backgroundScanner = new BackgroundScanner(
                    _outputPane!
                    );

                CodeLensConnectionHandler.RefreshAllCodeLensDataPointsAsync()
                    .FileAndForget(nameof(CodeLensConnectionHandler.RefreshAllCodeLensDataPointsAsync))
                    ;

                _backgroundScanner.AsyncStart();

                handler.RegisterTask(
                    Task.Run(
                        async () => await ShowProgressAsync(handler)
                        )
                    );
            }
            catch (Exception excp)
            {
                LogVS(excp);
            }
        }

        private async Task ShowProgressAsync(ITaskHandler handler)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            while (true)
            {
                await Task.Delay(100);

                _data.PercentComplete = (int)((_backgroundScanner?.Progress ?? 1f) * 100);
                handler.Progress.Report(_data);

                if (_backgroundScanner?.IsFinished ?? true)
                {
                    return;
                }
            }
        }
    }


    public class BackgroundScanner
    {
        private readonly IVsOutputWindowPane _outputPane;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly Thread _scanTask;

        public float Progress
        {
            get;
            private set;
        }

        public bool IsFinished
        {
            get;
            private set;
        }

        public SolutionBindContainer? SolutionBindContainer
        {
            get;
            private set;
        }

        public BackgroundScanner(
            IVsOutputWindowPane outputPane
            )
        {
            if (outputPane is null)
            {
                throw new ArgumentNullException(nameof(outputPane));
            }

            _outputPane = outputPane;
            _cancellationTokenSource = new CancellationTokenSource();

            _scanTask = new Thread(
                () => PerformScanBackground()
                );
        }

        public void AsyncStart()
        {
            _scanTask.Start();
        }

        public void SyncStop()
        {
            //stop scanning if it is working now
            _cancellationTokenSource.Cancel();

            //wait for scan has stopped
            _scanTask.Join();
        }

        private void PerformScanBackground(
            )
        {
            var token = _cancellationTokenSource.Token;

            SolutionBindContainer? sbc = null;
            try
            {
                //const int max = 100;
                //for (var cc = 0; cc <= max; cc++)
                //{
                //    if (token.IsCancellationRequested)
                //    {
                //        return;
                //    }

                //    Thread.Sleep(100);

                //    Progress = ((float)cc) / max;
                //}

                _outputPane!.OutputStringThreadSafe($"Dpdt scanning is started{Environment.NewLine}");

                var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

                var workspace = (Workspace)componentModel.GetService<VisualStudioWorkspace>();

                if (workspace == null)
                {
                    return;
                }

                if (!workspace.CurrentSolution.Projects.Any())
                {
                    return;
                }

                sbc = new SolutionBindContainer();
                var index = -1;
                var projectCount = workspace.CurrentSolution.Projects.Count();
                foreach (var project in workspace.CurrentSolution.Projects)
                {
                    Progress = ((float)(++index)) / projectCount;

                    if (token.IsCancellationRequested)
                    {
                        _outputPane!.OutputStringThreadSafe($"Dpdt scanning is cancelled{Environment.NewLine}");

                        break;
                    }

                    if (project == null)
                    {
                        continue;
                    }

                    if (!project.SupportsCompilation)
                    {
                        continue;
                    }

                    if (project.Language != "C#")
                    {
                        continue;
                    }

                    var letsgo = false;
                    foreach (var reference in project.AnalyzerReferences)
                    {
                        if (reference.Display == "DpdtInject.Generator")
                        {
                            letsgo = true;

                            break;
                        }
                    }

                    if (!letsgo)
                    {
                        continue;
                    }

                    var swt = Stopwatch.StartNew();

                    Compilation? compilation = null;
                    ThreadHelper.JoinableTaskFactory.Run(
                        async () =>
                        {
                            try
                            {
                                compilation = await project.GetCompilationAsync(token);
                            }
                            catch (OperationCanceledException ope)
                            {
                                //ok
                            }
                            catch (Exception excp)
                            {
                                LogVS(excp);
                            }
                        });

                    //first check for cancellation...
                    if (token.IsCancellationRequested)
                    {
                        _outputPane!.OutputStringThreadSafe($"Dpdt scanning is cancelled{Environment.NewLine}");

                        break;
                    }

                    //..next for compilation errors, because both will result with compilation == null
                    if (compilation == null)
                    {
                        continue;
                    }

                    _outputPane!.OutputStringThreadSafe($"Compilation {project.Name} taken :{swt.Elapsed}{Environment.NewLine}");

                    var diag = compilation.GetDiagnostics();

                    var errors = diag.Where(j => j.Severity == DiagnosticSeverity.Error).ToList();
                    if (errors.Count > 0)
                    {
                        var errorMessage = string.Join(
                            Environment.NewLine,
                            errors.Select(j => j.ToString())
                            );

                        //DpdtPackage.ShowMessageBox(
                        //    "Error has been found",
                        //    errorMessage
                        //    );

                        _outputPane!.OutputStringThreadSafe($"Compilation error:{Environment.NewLine}");
                        _outputPane!.OutputStringThreadSafe(errorMessage);
                        _outputPane!.OutputStringThreadSafe(Environment.NewLine);

                        continue;
                    }

                    swt.Restart();

                    var typeInfoContainer = new ExtensionTypeInfoContainer(
                        compilation
                        );

                    var diagnosticReporter = new DebugDiagnosticReporter(
                        );

                    var internalGenerator = new DpdtInternalGenerator(
                        diagnosticReporter,
                        false
                        );

                    var clusterBindings = internalGenerator.DoExtraction(
                        typeInfoContainer
                        );

                    _outputPane!.OutputStringThreadSafe($"Binding extraction from {project.Name} taken :{swt.Elapsed}{Environment.NewLine}");

                    if (token.IsCancellationRequested)
                    {
                        _outputPane!.OutputStringThreadSafe($"Dpdt scanning is cancelled{Environment.NewLine}");

                        break;
                    }

                    var pbc = new ProjectBindContainer(project);
                    foreach (var clusterBinding in clusterBindings)
                    {
                        var cbc = new ClusterBindContainer(clusterBinding.ClusterType);
                        foreach (var methodBinding in clusterBinding.MethodBindings)
                        {
                            cbc.Add(
                                new MethodBindContainer(
                                    clusterBinding.ClusterType,
                                    methodBinding.Item1,
                                    methodBinding.Item2
                                    )
                                );
                        }

                        pbc.Add(cbc);
                    }

                    sbc.Add(pbc);
                }
            }
            catch (Exception excp)
            {
                LogVS(excp);
            }
            finally
            {
                if (sbc != null && sbc.Dict.Count > 0)
                {
                    this.SolutionBindContainer = sbc;
                }

                _outputPane!.OutputStringThreadSafe($"Dpdt scanning is finished{Environment.NewLine}");

                // CodeLenses usually only live as long as the document is open so we just refresh all the connected ones.
                //they will show actual status
                CodeLensConnectionHandler.RefreshAllCodeLensDataPointsAsync()
                    .FileAndForget(nameof(PerformScanBackground))
                    ;

                Progress = 1f;
                IsFinished = true;
            }
        }

    }

    public class SolutionBindContainer
    {
        private readonly Dictionary<string, ProjectBindContainer> _dict = new Dictionary<string, ProjectBindContainer>();

        public IReadOnlyDictionary<string, ProjectBindContainer> Dict => _dict;

        public SolutionBindContainer(
            )
        {
        }

        public void Add(
            ProjectBindContainer pbc
            )
        {
            if (pbc is null)
            {
                throw new ArgumentNullException(nameof(pbc));
            }

            _dict[pbc.Project.Name] = pbc;
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool TryGetBindingContainer(
            Guid bindingIdentifier,
            out IBindingContainer? resultBindingContainer
            )
        {
            foreach (var ppair in Dict)
            {
                var projectBind = ppair.Value;

                foreach (var cpair in projectBind.DictByDisplayString)
                {
                    var clusterBind = cpair.Value;

                    foreach (var mpair in clusterBind.Dict)
                    {
                        var methodBind = mpair.Value;

                        foreach (var bindingContainer in methodBind.BindingContainers)
                        {
                            if (bindingContainer.Identifier == bindingIdentifier)
                            {
                                resultBindingContainer = bindingContainer;

                                return true;
                            }
                        }
                    }
                }
            }


            resultBindingContainer = null;
            return false;
        }
    }

    public class ProjectBindContainer
    {
        private readonly Dictionary<string, ClusterBindContainer> _dictByDisplayString = new Dictionary<string, ClusterBindContainer>();
        private readonly Dictionary<string, ClusterBindContainer> _dictByFullyQualifiedName = new Dictionary<string, ClusterBindContainer>();

        public Project Project
        {
            get;
        }
        public IReadOnlyDictionary<string, ClusterBindContainer> DictByDisplayString => _dictByDisplayString;
        public IReadOnlyDictionary<string, ClusterBindContainer> DictByFullyQualifiedName => _dictByFullyQualifiedName;

        public ProjectBindContainer(
            Project project
            )
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            Project = project;
        }

        public void Add(
            ClusterBindContainer cbc
            )
        {
            if (cbc is null)
            {
                throw new ArgumentNullException(nameof(cbc));
            }

            _dictByDisplayString[cbc.ClusterType.ToDisplayString()] = cbc;
            _dictByFullyQualifiedName[cbc.ClusterType.GetFullyQualifiedName()] = cbc;
        }
    }

    public class ClusterBindContainer
    {
        private readonly Dictionary<string, MethodBindContainer> _dict =  new Dictionary<string, MethodBindContainer>();

        public INamedTypeSymbol ClusterType
        {
            get;
        }

        public IReadOnlyDictionary<string, MethodBindContainer> Dict => _dict;


        public ClusterBindContainer(
            INamedTypeSymbol clusterType
            )
        {
            if (clusterType is null)
            {
                throw new ArgumentNullException(nameof(clusterType));
            }

            ClusterType = clusterType;
        }

        public void Add(
            MethodBindContainer mbc
            )
        {
            if (mbc is null)
            {
                throw new ArgumentNullException(nameof(mbc));
            }

            _dict[mbc.MethodSyntax.Identifier.Text] = mbc;
        }
    }

    public class MethodBindContainer
    {
        public INamedTypeSymbol ClusterType
        {
            get;
        }

        public MethodDeclarationSyntax MethodSyntax
        {
            get;
        }

        public IReadOnlyList<IBindingContainer> BindingContainers
        {
            get;
        }

        public MethodBindContainer(
            INamedTypeSymbol clusterType,
            MethodDeclarationSyntax methodSyntax,
            IReadOnlyList<IBindingContainer> bindingContainers
            )
        {
            if (clusterType is null)
            {
                throw new ArgumentNullException(nameof(clusterType));
            }

            if (methodSyntax is null)
            {
                throw new ArgumentNullException(nameof(methodSyntax));
            }

            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            ClusterType = clusterType;
            MethodSyntax = methodSyntax;
            BindingContainers = bindingContainers;
        }

    }

}
