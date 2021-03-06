using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices;
using System.Linq;

namespace DpdtInject.Extension.ExtensionStatus.FullyLoaded
{
    [Export(typeof(FullyLoadedStatusContainer))]
    public sealed class FullyLoadedStatusContainer : IDisposable
    {
        private readonly ExtensionStatusContainer _esc;

        public delegate void SolutionStatusChangedDelegate(
            bool actualSolutionStatus
            );

        private bool _isSolutionFullyLoaded;
        private bool _solutionStatus;

        private readonly IVsTask _task;
        private long _disposed = 0L;

        public bool SolutionStatus => _solutionStatus;

        public event SolutionStatusChangedDelegate? SolutionStatusChangedEvent;

        [ImportingConstructor]
        public FullyLoadedStatusContainer(
            ExtensionStatusContainer esc
            )
        {
            if (esc is null)
            {
                throw new ArgumentNullException(nameof(esc));
            }

            _esc = esc;

            _task = ThreadHelper.JoinableTaskFactory.RunAsyncAsVsTask<int>(
                VsTaskRunContext.UIThreadBackgroundPriority,
                ReadFullyLoadStatusAsync
                );
        }
        
        public void SyncStop()
        {
            if (Interlocked.Exchange(ref _disposed, 1L) != 0L)
            {
                return;
            }

            _task.Cancel();
        }

        public void Dispose()
        {
            SyncStop();
        }

        private readonly Dictionary<string, TaskCompletionSource<bool>> _taskCompletionSources = new Dictionary<string, TaskCompletionSource<bool>>();

        private async System.Threading.Tasks.Task<int> ReadFullyLoadStatusAsync(
            CancellationToken token
            )
        {
            while (!token.WaitHandle.WaitOne(100))
            {
                //await Task.Delay(100, token);

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(token);

                var solution = (AsyncPackage.GetGlobalService(typeof(SVsSolution)) as IVsSolution)!;

                solution.GetProperty((int)__VSPROPID4.VSPROPID_IsSolutionFullyLoaded, out object asm);

                await TaskScheduler.Default;

                if (!(asm is bool))
                {
                    continue;
                }

                //var context = Microsoft.VisualStudio.Shell.KnownUIContexts.SolutionExistsAndFullyLoadedContext;
                //var contextActive = context.IsActive;

                _isSolutionFullyLoaded = (bool) asm;
                var solutionStatus = _isSolutionFullyLoaded && _esc.SolutionExists;
                
                if (_solutionStatus != solutionStatus)
                {
                    _solutionStatus = solutionStatus;

                    OnSolutionStatusChanged(solutionStatus);
                }
            }

            return VSConstants.S_OK;
        }

        private static List<Microsoft.CodeAnalysis.Diagnostics.AnalyzerReference> GetAR()
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var workspace = (Workspace)componentModel.GetService<VisualStudioWorkspace>();
            var project = workspace.CurrentSolution.Projects.First();
            var ar = project.AnalyzerReferences.ToList();

            return ar;
        }

        private void OnSolutionStatusChanged(
            bool actualSolutionStatus
            )
        {
            SolutionStatusChangedEvent?.Invoke(actualSolutionStatus);
        }

    }
}
