using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using DpdtInject.Extension.Container;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension
{
    //[Export(typeof(WaitRescanner))]
    //public class WaitRescanner //: IDisposable
    //{
    //    private readonly ContainerAndScanner _containerAndScanner;

    //    private readonly AutoResetEvent _changeSignal = new AutoResetEvent(false);

    //    private long _timeToScan = DateTime.MaxValue.Ticks;
        
    //    [ImportingConstructor]
    //    public WaitRescanner(
    //        ContainerAndScanner containerAndScanner
    //        )
    //    {
    //        if (containerAndScanner is null)
    //        {
    //            throw new ArgumentNullException(nameof(containerAndScanner));
    //        }

    //        _containerAndScanner = containerAndScanner;

    //        ThreadHelper.JoinableTaskFactory.RunAsync(
    //            async () => await CheckForStateAsync()
    //            ).FileAndForget(nameof(WaitRescanner));
    //    }

    //    public void NewChangesIncomes(
    //        )
    //    {
    //        _timeToScan = DateTime.Now.AddSeconds(10).Ticks;
    //        _changeSignal.Set();
    //    }

    //    //public void Dispose()
    //    //{
    //    //    _changeSignal.Set();
    //    //    _changeSignal.Dispose();
    //    //}

    //    private async Task CheckForStateAsync(
    //        )
    //    {
    //        //switch to background thread and work in it all the time
    //        await TaskScheduler.Default;

    //        while (true)
    //        {
    //            if (_changeSignal.WaitOne(100))
    //            {
    //                //something happens! source has been changed!
    //                //stop running scan if exists, because we are compiling the old sources
    //                _containerAndScanner.StopScanSync();
    //            }

    //            //regardless of signal firing,
    //            //check for time to start scanning comes
    //            var storedTimeToScan = _timeToScan;
    //            if (new DateTime(storedTimeToScan) < DateTime.Now)
    //            {
    //                //it's time to (re)start scan

    //                _containerAndScanner.AsyncStartScan();

    //                //no need to scan more, until new changes incomes
    //                Interlocked.CompareExchange(ref _timeToScan, DateTime.MaxValue.Ticks, storedTimeToScan);
    //            }
    //        }
    //    }
    //}
}
