using DpdtInject.Extension.Container;
using Microsoft.VisualStudio.OperationProgress;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Extension.ExtensionStatus
{
    [Export(typeof(IntellisenseStatusContainer))]
    public class IntellisenseStatusContainer
    {
        private readonly ContainerAndScanner _containerAndScanner;

        [ImportingConstructor]
        public IntellisenseStatusContainer(
            ContainerAndScanner containerAndScanner
            )
        {
            if (containerAndScanner is null)
            {
                throw new ArgumentNullException(nameof(containerAndScanner));
            }

            _containerAndScanner = containerAndScanner;

            var operationProgress = Package.GetGlobalService(typeof(SVsOperationProgress)) as IVsOperationProgress2;
            if (operationProgress != null)
            {
                var opss = operationProgress as IVsOperationProgressStatusService;

                if (opss != null)
                {
                    var intelliSenseStageStatus = opss.GetStageStatus(CommonOperationProgressStageIds.Intellisense);
                    intelliSenseStageStatus.InProgressChanged += IntelliSenseStatus_InProgressChanged;
                }
            }
        }

        private void IntelliSenseStatus_InProgressChanged(object sender, OperationProgressStatusChangedEventArgs e)
        {
            if (!e.Status.IsInProgress)
            {
                _containerAndScanner.AsyncStartScan();
            }
        }

    }
}
