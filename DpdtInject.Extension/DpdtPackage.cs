using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using DpdtInject.Extension.CodeLens;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.ExtensionStatus;
using DpdtInject.Extension.ExtensionStatus.FullyLoaded;
using DpdtInject.Extension.Options;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using static DpdtInject.Extension.Shared.Logging;

namespace DpdtInject.Extension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuid)]
    [ProvideBindingPath]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(DialogPageProvider.General), "Dpdt", "General", 0, 0, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class DpdtPackage : AsyncPackage
    {
        public const string DpdtNugetPackageName = "Dpdt.Injector";

        public const string PackageGuid = "d53ea3df-798b-402f-b6fd-0dc4544a4be7";

        /// <summary>
        /// Initializes a new instance of the <see cref="DpdtPackage"/> class.
        /// </summary>
        public DpdtPackage(
            )
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress
            )
        {
            LogVS("Start 1");

            await base.InitializeAsync(cancellationToken, progress);

            LogVS("Start 2");

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await DpdtInstallCommand.InitializeAsync(this);
            await DoCreateClusterCommandId.InitializeAsync(this);

            LogVS("Start 3");

            //no, absense of await here is by design! we do not wait it for its completion.
            _ = CodeLensConnectionHandler.AcceptCodeLensConnectionsAsync();

            LogVS("Start 4");

            //var dte = GetService(typeof(EnvDTE.DTE)) as DTE2;
            var container = (await this.GetServiceAsync(typeof(SComponentModel)) as IComponentModel)!;

            LogVS("Start 5");

            var solution = await this.GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            if (solution != null)
            {
                LogVS("Start 6");

                var sEventsExt0 = container.GetService<ExtensionStatusContainer>();
                foreach (var sEventExt in new[] { sEventsExt0 })
                {
                    uint cookie;
                    solution!.AdviseSolutionEvents(sEventExt, out cookie);

                    sEventExt.Cookie = cookie;
                }
            }

            LogVS("Start 7");

            var isc = container!.GetService<IntellisenseStatusContainer>();
            await isc.StartAsync();

            isc.StartScanningInThreadPool();

            LogVS("Start 8");
        }

        #endregion

        internal static void ShowMessageBox(
            string title,
            string message
            )
        {
            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                ServiceProvider.GlobalProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

    }
}
