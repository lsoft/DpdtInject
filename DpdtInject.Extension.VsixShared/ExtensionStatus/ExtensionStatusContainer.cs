using System;
using System.ComponentModel.Composition;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using static DpdtInject.Extension.Shared.Logging;

namespace DpdtInject.Extension.ExtensionStatus
{
    [Export(typeof(ExtensionStatusContainer))]
    public class ExtensionStatusContainer : IVsSolutionEventsExt
    {
        private readonly DTE2 _dte;

        public bool SolutionExists
        {
            get;
            private set;
        }

        public uint Cookie
        {
            get;
            set;
        }


        [ImportingConstructor]
        public ExtensionStatusContainer(
            )
        {
            _dte = (AsyncPackage.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2)!;

            ProcessStatus();
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            ProcessStatus();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        private void ProcessStatus()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                try
                {
                    SolutionExists = DetermineIsSolutionExists();
                }
                catch (Exception excp)
                {
                    LogVS(excp);
                }
            }).FileAndForget(nameof(ProcessStatus));

        }

        private bool DetermineIsSolutionExists()
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(DetermineIsSolutionExists));

            if (_dte.Solution == null || string.IsNullOrEmpty(_dte.Solution.FullName) || _dte.Solution.Projects.Count == 0)
            {
                return false;
            }

            return true;
        }


    }

}
