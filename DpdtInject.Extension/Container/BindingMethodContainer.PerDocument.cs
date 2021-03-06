//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using DpdtInject.Extension.Container.Component;
//using DpdtInject.Extension.ExtensionStatus;
//using DpdtInject.Extension.Helper;
//using DpdtInject.Extension.Shared.Dto;
//using DpdtInject.Generator;
//using DpdtInject.Generator.BindExtractor;
//using DpdtInject.Generator.BindExtractor.Parsed;
//using DpdtInject.Generator.Helpers;
//using DpdtInject.Injector.Compilation;
//using Microsoft;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.ComponentModelHost;
//using Microsoft.VisualStudio.LanguageServices;
//using Microsoft.VisualStudio.Shell;
//using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio.TaskStatusCenter;
//using Microsoft.VisualStudio.Threading;
//using Task = System.Threading.Tasks.Task;

//namespace DpdtInject.Extension.Container
//{
//    [Export(typeof(BindingMethodContainer))]
//    public class BindingMethodContainer
//    {
//        private readonly object _locker = new object();

//        private readonly ExtensionStatusContainer _esc;
//        private readonly List<IDpdtClusterDetail> _details = new List<IDpdtClusterDetail>();
        
//        private IVsTask? _task;
//        private TaskProgressData _data;

//        [ImportingConstructor]
//        public BindingMethodContainer(
//            ExtensionStatusContainer esc
//            )
//        {
//            if (esc is null)
//            {
//                throw new ArgumentNullException(nameof(esc));
//            }

//            _esc = esc;

//            _esc.SolutionStatusChangedEvent += OnSolutionStatusChanged;

//            //force to start scanning when starting
//            OnSolutionStatusChanged(true);
//        }

//        private void OnSolutionStatusChanged(
//            bool actualSolutionStatus
//            )
//        {
//            if (!actualSolutionStatus)
//            {
//                //no solution active and fully loaded

//                lock (_locker)
//                {
//                    //stop scanning if it is working now
//                    _task?.Cancel();
//                    _task = null;
//                }

//                _details.Clear();
                
//                return;
//            }

//            //there is a ready solution!
//            //start scanning

//            lock (_locker)
//            {
//                _task?.Cancel();

//                _task = ThreadHelper.JoinableTaskFactory.RunAsyncAsVsTask(
//                    VsTaskRunContext.UIThreadBackgroundPriority,
//                    StartScanAsync
//                    );
//            }
//        }

//        private async Task<int> StartScanAsync(
//            CancellationToken token
//            )
//        {
//            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(token);

//            var taskStatusCenterService = (AsyncPackage.GetGlobalService(typeof(SVsTaskStatusCenterService)) as IVsTaskStatusCenterService)!;
//            Assumes.Present(taskStatusCenterService);

//            var options = default(TaskHandlerOptions);
//            options.Title = "Scanning solution for Dpdt clusters and its bindings...";
//            options.ActionsAfterCompletion = CompletionActions.None;

//            _data = default(TaskProgressData);
//            _data.CanBeCanceled = false;

//            var handler = taskStatusCenterService.PreRegister(options, _data);
//            handler.RegisterTask(
//                Task.Run(
//                    async () => await DoScan2Async(handler, token)
//                    )
//                );

//            return VSConstants.S_OK;
//        }

//        private async Task DoScan2Async(
//            ITaskHandler handler,
//            CancellationToken token
//            )
//        {
//            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
//            var workspace = (Workspace)componentModel.GetService<VisualStudioWorkspace>();

//            if (workspace == null)
//            {
//                return;
//            }

//            if (!workspace.CurrentSolution.Projects.Any())
//            {
//                return;
//            }

//            var totalDocumentCount = workspace.CurrentSolution.Projects.Sum(p => p?.Documents?.Count());
//            var currentDocumentIndex = 0;

//            void ShowProgress()
//            {
//                _data.PercentComplete = currentDocumentIndex * 100 / totalDocumentCount;
//                handler.Progress.Report(_data);
//            }

//            var semanticModels = new List<SemanticModel>();
//            var bindMethods = new List<(INamedTypeSymbol, BaseMethodDeclarationSyntax)>();
//            foreach (var project in workspace.CurrentSolution.Projects)
//            {
//                if (project == null)
//                {
//                    continue;
//                }

//                ShowProgress();

//                if (!project.SupportsCompilation)
//                {
//                    currentDocumentIndex += project.Documents.Count();
//                    continue;
//                }
//                if (project.Language != "C#")
//                {
//                    currentDocumentIndex += project.Documents.Count();
//                    continue;
//                }

//                var letsgo = false;
//                foreach (var reference in project.AnalyzerReferences)
//                {
//                    if (reference.Display == "DpdtInject.Generator")
//                    {
//                        letsgo = true;

//                        break;
//                    }
//                }

//                if (!letsgo)
//                {
//                    currentDocumentIndex += project.Documents.Count();
//                    continue;
//                }

//                //if (!string.IsNullOrEmpty(project.FilePath))
//                //{
//                //    var projectFileBody = await project.FilePath!.ReadAllTextAsync();

//                //    if (!projectFileBody.Contains("Dpdt.Injector"))
//                //    {
//                //        //this project does not include Dpdt, skip it
//                //        currentDocumentIndex += project.Documents.Count();
//                //        continue;
//                //    }
//                //}


//                foreach (var document in project.Documents)
//                {
//                    if (document == null)
//                    {
//                        currentDocumentIndex++;
//                        continue;
//                    }

//                    ShowProgress();

//                    if (!document.SupportsSemanticModel)
//                    {
//                        currentDocumentIndex++;
//                        continue;
//                    }

//                    if (!document.SupportsSyntaxTree)
//                    {
//                        currentDocumentIndex++;
//                        continue;
//                    }

//                    var documentText = await document.GetTextAsync(token);
//                    if (documentText == null)
//                    {
//                        currentDocumentIndex++;
//                        continue;
//                    }

//                    var body = documentText.ToString();

//                    if (!body.Contains("Dpdt"))
//                    {
//                        currentDocumentIndex++;
//                        continue;
//                    }

//                    if (!body.Contains("DpdtBindingMethod"))
//                    {
//                        currentDocumentIndex++;
//                        continue;
//                    }

//                    var root = await document.GetSyntaxRootAsync(token);

//                    if (root == null)
//                    {
//                        currentDocumentIndex++;
//                        continue;
//                    }

//                    var semanticModel = await document.GetSemanticModelAsync(token);

//                    if (semanticModel is null)
//                    {
//                        currentDocumentIndex++;
//                        continue;
//                    }

//                    var classDeclarationSyntaxes = root
//                        .DescendantNodes()
//                        .OfType<ClassDeclarationSyntax>()
//                        .ToList()
//                        ;

//                    foreach (var classDeclarationSyntax in classDeclarationSyntaxes)
//                    {
//                        var clusterType = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);

//                        if (clusterType is null)
//                        {
//                            continue;
//                        }
//                        проверить что этот класс еще не обрабатывали

//                        if (!RoslynHelper.IsClusterType(clusterType)) //do not switch to `clusterType.IsClusterType()` - this will broke extension somehow!! yes yes!!
//                        {
//                            continue;
//                        }

//                        clusterType.ScanForRequiredSyntaxes(
//                            out var bindMethodSyntaxes,
//                            out var compilationUnitSyntaxes
//                            );

//                        foreach (var bindMethodSyntax in bindMethodSyntaxes)
//                        {
//                            bindMethods.Add((clusterType, bindMethodSyntax));
//                        }
//                    }

//                    //DefaultTypeScanner
//                    //var cus = SyntaxFactory.ParseCompilationUnit(
//                    //    body
//                    //    );


//                    semanticModels.Add(semanticModel);

//                    currentDocumentIndex++;
//                }

//            }

//            var semanticModelDecorator = new SemanticModelDecorator(
//                semanticModels
//                );

//            var typeInfoContainer = new ExtensionTypeInfoContainer(
//                semanticModelDecorator
//                );

//            //var ig = new DpdtInternalGenerator(
//            //    new DebugDiagnosticReporter()
//            //    );
//            //ig.Execute(
//            //    typeInfoContainer
//            //    );

//            var bindExtractor = new DefaultBindExtractor(
//                semanticModelDecorator,
//                new ParsedBindExpressionFactory(
//                    typeInfoContainer,
//                    semanticModelDecorator,
//                    new ConstructorArgumentFromSyntaxExtractor(
//                        semanticModelDecorator
//                        ),
//                    new ConstructorArgumentDetector(
//                        new BindConstructorChooser()
//                        )
//                    )
//                );

//            foreach (var bindMethod in bindMethods)
//            {
//                bindExtractor.Visit(bindMethod.Item2);
//            }

//            var clusterBindings = bindExtractor.GetClusterBindings(
//                bindMethods[0].Item1
//                );


//            for (var cc = 0; cc < 100; cc++)
//            {
//                if (token.IsCancellationRequested)
//                {
//                    break;
//                }


//                await Task.Delay(100, token);
//                //await Task.Yield();

//                _data.PercentComplete = cc;
//                handler.Progress.Report(_data);
//            }

//            //return VSConstants.S_OK;
//            //ThreadHelper.ThrowIfOnUIThread(nameof(InternalProcessStatus));
//        }
//    }
//}
