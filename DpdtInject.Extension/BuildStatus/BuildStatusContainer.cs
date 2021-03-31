using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Extension.BuildStatus
{
    [Export(typeof(BuildStatusContainer))]
    public sealed class BuildStatusContainer
    {
        private readonly DTE2 _dte;
        private readonly BuildEvents _buildEvents;

        public bool BuildIsInProgress
        {
            get;
            private set;
        }

        public event ChangeBuildStatusDelegate? ChangeBuildStatusEvent;

        [ImportingConstructor]
        public BuildStatusContainer(
            )
        {
            _dte = (AsyncPackage.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2)!;
            _buildEvents = _dte.Events.BuildEvents;
        }

        public void Subscribe()
        {
            _buildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;
            _buildEvents.OnBuildDone += BuildEvents_OnBuildDone;
        }

        private void BuildEvents_OnBuildBegin(EnvDTE.vsBuildScope Scope, EnvDTE.vsBuildAction Action)
        {
            BuildIsInProgress = true;
            ChangeBuildStatusEvent?.Invoke(true);
        }

        private void BuildEvents_OnBuildDone(EnvDTE.vsBuildScope Scope, EnvDTE.vsBuildAction Action)
        {
            BuildIsInProgress = false;
            ChangeBuildStatusEvent?.Invoke(false);
        }

    }

    public delegate void ChangeBuildStatusDelegate(bool buildIsInProgressNow);
}
