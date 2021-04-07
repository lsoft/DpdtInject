using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using DpdtInject.Extension.CodeLens;
using DpdtInject.Extension.Container.Component;
using DpdtInject.Generator.Core;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TaskStatusCenter;
using Microsoft.VisualStudio.Threading;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using Task = System.Threading.Tasks.Task;
using Project = Microsoft.CodeAnalysis.Project;
using Thread = System.Threading.Thread;

using static DpdtInject.Extension.Shared.Logging;
using DpdtInject.Extension.BuildStatus;
using DpdtInject.Generator.Core.Binding.Xml;
using DpdtInject.Generator.Core.Meta;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector.Helper;

namespace DpdtInject.Extension.Container
{
    [Export(typeof(ContainerAndScanner))]
    public class ContainerAndScanner
    {
        private readonly object _locker = new object();
        private readonly BuildStatusContainer _buildStatusContainer;
        private IVsOutputWindowPane? _outputPane;

        private TaskProgressData _data;
        private BackgroundScanner? _backgroundScanner;

        public ISolutionBindContainer? Binds => _backgroundScanner?.SolutionBindContainer;

        [ImportingConstructor]
        public ContainerAndScanner(
            BuildStatusContainer buildStatusContainer
            )
        {
            if (buildStatusContainer is null)
            {
                throw new ArgumentNullException(nameof(buildStatusContainer));
            }

            _buildStatusContainer = buildStatusContainer;
            _buildStatusContainer.ChangeBuildStatusEvent += buildStatusContainer_ChangeBuildStatusEvent;
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

        private void buildStatusContainer_ChangeBuildStatusEvent(bool buildIsInProgressNow)
        {
            AsyncStartScan();
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
                    _outputPane!,
                    _buildStatusContainer
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
        private readonly BuildStatusContainer _buildStatusContainer;
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

        public ISolutionBindContainer? SolutionBindContainer
        {
            get;
            private set;
        }

        public BackgroundScanner(
            IVsOutputWindowPane outputPane,
            BuildStatusContainer buildStatusContainer
            )
        {
            if (outputPane is null)
            {
                throw new ArgumentNullException(nameof(outputPane));
            }

            if (buildStatusContainer is null)
            {
                throw new ArgumentNullException(nameof(buildStatusContainer));
            }

            _outputPane = outputPane;
            _buildStatusContainer = buildStatusContainer;
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

            SolutionBindContainerXml? sbc = null;
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

                if (_buildStatusContainer.BuildIsInProgress)
                {
                    _outputPane!.OutputStringThreadSafe($"Dpdt scanning is cancelled due to build/cleanup is in progress{Environment.NewLine}");
                    return;
                }

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

                sbc = new SolutionBindContainerXml();
                var index = -1;
                var projectCount = workspace.CurrentSolution.Projects.Count();
                foreach (var project in workspace.CurrentSolution.Projects)
                {
                    Progress = ((float)(++index)) / projectCount;

                    if (token.IsCancellationRequested)
                    {
                        _outputPane!.OutputStringThreadSafe($"  Dpdt scanning is cancelled{Environment.NewLine}");
                        break;
                    }

                    if (_buildStatusContainer.BuildIsInProgress)
                    {
                        _outputPane!.OutputStringThreadSafe($"  Dpdt scanning is cancelled due to build/cleanup is in progress{Environment.NewLine}");
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
                        //surrogate marker for nuget Dpdt.Injector has installed to the project
                        if (reference.Display == "DpdtInject.Generator")
                        {
                            letsgo = true;

                            break;
                        }
                    }

                    if (!letsgo)
                    {
                        _outputPane!.OutputStringThreadSafe($"  {project.Name}: Project skipped due to absense of Dpdt package installed{Environment.NewLine}");
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
                        _outputPane!.OutputStringThreadSafe($"  Dpdt scanning is cancelled{Environment.NewLine}");

                        break;
                    }

                    //..next for compilation errors, because both will result with compilation == null
                    if (compilation == null)
                    {
                        continue;
                    }

                    _outputPane!.OutputStringThreadSafe($"    {project.Name}: compilation taken {swt.Elapsed}{Environment.NewLine}");

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

                        _outputPane!.OutputStringThreadSafe($"     {project.Name}: compilation error {Environment.NewLine}");
                        _outputPane!.OutputStringThreadSafe("     " + errorMessage);
                        _outputPane!.OutputStringThreadSafe(Environment.NewLine);

                        //do not skip this project analysis, it may be partially fine
                        //continue;
                    }

                    swt.Restart();

                    var meta = new BuiltinMeta();
                    if(!meta.TryExtract(compilation, out var projectXml))
                    {
                        continue;
                    }
                    sbc.Append(projectXml!);

                    _outputPane!.OutputStringThreadSafe($"    {project.Name}: Binding extraction taken {swt.Elapsed}{Environment.NewLine}");
                    _outputPane!.OutputStringThreadSafe($"    {project.Name}: Found {projectXml.TotalBindingCount} binding{Environment.NewLine}");

                    if (token.IsCancellationRequested)
                    {
                        _outputPane!.OutputStringThreadSafe($"  Dpdt scanning is cancelled{Environment.NewLine}");

                        break;
                    }
                }
            }
            catch (Exception excp)
            {
                LogVS(excp);
                _outputPane!.OutputStringThreadSafe($"  Dpdt scanning failed: {excp.Message}{Environment.NewLine}");
                _outputPane!.OutputStringThreadSafe($"  Dpdt scanning failed: {excp.StackTrace}{Environment.NewLine}");
            }
            finally
            {
                if (sbc != null)
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
}
