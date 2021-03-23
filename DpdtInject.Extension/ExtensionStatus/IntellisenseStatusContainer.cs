using DpdtInject.Extension.Container;
using Microsoft.VisualStudio.OperationProgress;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Composition;
using Task = System.Threading.Tasks.Task;

using static DpdtInject.Extension.Shared.Logging;
using DpdtInject.Extension.ExtensionStatus.FullyLoaded;

namespace DpdtInject.Extension.ExtensionStatus
{
    [Export(typeof(IntellisenseStatusContainer))]
    public class IntellisenseStatusContainer
    {
        private readonly object _locker = new object();
        private readonly FullyLoadedStatusContainer _flsc;
        private readonly ContainerAndScanner _containerAndScanner;

        [ImportingConstructor]
        public IntellisenseStatusContainer(
            FullyLoadedStatusContainer flsc,
            ContainerAndScanner containerAndScanner
            )
        {
            if (flsc is null)
            {
                throw new ArgumentNullException(nameof(flsc));
            }

            if (containerAndScanner is null)
            {
                throw new ArgumentNullException(nameof(containerAndScanner));
            }
            _flsc = flsc;
            _containerAndScanner = containerAndScanner;
        }

        public async Task StartAsync()
        {
            await _containerAndScanner.InitializeAsync();

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

            _flsc.SolutionStatusChangedEvent += OnSolutionStatusChanged;
        }

        private void IntelliSenseStatus_InProgressChanged(object sender, OperationProgressStatusChangedEventArgs e)
        {
            if (!e.Status.IsInProgress)
            {
                StartScanningInThreadPool();
            }
        }

        private void OnSolutionStatusChanged(
            bool actualSolutionStatus
            )
        {

            if (!actualSolutionStatus)
            {
                //no solution active and fully loaded

                DoingInThreadPool(
                    () =>
                    {
                        //doing sync stop
                        _containerAndScanner.StopScanSync();
                    }
                    );
            }
            else
            {
                //there is a ready solution!

                DoingInThreadPool(
                    () =>
                    {
                        //doing sync stop
                        _containerAndScanner.StopScanSync();

                        //start scanning
                        _containerAndScanner.AsyncStartScan();
                    }
                    );
            }
        }

        public void StartScanningInThreadPool(
            )
        {
            DoingInThreadPool(
                () => _containerAndScanner.AsyncStartScan()
                );
        }

        public void DoingInThreadPool(
            Action doAction
            )
        {
            if (doAction is null)
            {
                throw new ArgumentNullException(nameof(doAction));
            }

            try
            {
                Task.Run(() =>
                {
                    lock (_locker)
                    {
                        doAction();
                    }
                }).FileAndForget(nameof(DoingInThreadPool));
            }
            catch (Exception excp)
            {
                LogVS(excp);
            }
        }
    }
}
