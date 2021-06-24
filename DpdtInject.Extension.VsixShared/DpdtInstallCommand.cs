using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Options;
using DpdtInject.Extension.Shared;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension
{
    public class DpdtInstallCommand
    {
        public static string ProjectKind = "{52AEFF70-BBD8-11d2-8598-006097C68E81}";

        private static IComponentModel? _componentModel;

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0xC100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("A30C3B4B-537C-4EEF-A7FF-232D47063286");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage _package;

        private bool _isInProgress = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="DpdtInstallCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private DpdtInstallCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this._package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(Execute, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DpdtInstallCommand? Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return _package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            _componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new DpdtInstallCommand(package, commandService!);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            _isInProgress = true;

            ThreadHelper.JoinableTaskFactory.Run(
                $"Install {DpdtPackage.DpdtNugetPackageName} Nuget package",
                async (progress) =>
                {
                    try
                    {
                        var componentModel = await _package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
                        if (componentModel == null)
                        {
                            return;
                        }

                        var dte = await _package.GetServiceAsync(typeof(DTE)) as DTE2;
                        if (dte == null)
                        {
                            Logging.LogVS("Failed to get DTE service.");
                            return;
                        }

                        if (!dte.TryGetSelectedProject(out var envProject))
                        {
                            return;
                        }

                        try
                        {
                            progress.Report(
                                new ThreadedWaitDialogProgressData(
                                    $"Installing {DpdtPackage.DpdtNugetPackageName} Nuget package, this may take a minute..."
                                    )
                                );

                            //await Task.Delay(10000);

                            var packageInstaller = componentModel.GetService<IVsPackageInstaller2>();
                            packageInstaller.InstallLatestPackage(null, envProject, DpdtPackage.DpdtNugetPackageName, true, false);
                        }
                        catch (Exception excp)
                        {
                            Logging.LogVS(excp);
                            ShowError(excp.Message + Environment.NewLine + excp.StackTrace);
                        }
                    }
                    finally
                    {
                        _isInProgress = false;
                    }

            });
        }


        private void ShowError(string errorMessage)
        {
            VsShellUtilities.ShowMessageBox(
                _package,
                errorMessage,
                $"Error has been found",
                OLEMSGICON.OLEMSGICON_WARNING,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is OleMenuCommand omc))
                {
                    return;
                }

                if (!GeneralOptions.Instance.Enabled)
                {
                    omc.Visible = false;
                    return;
                }

                if (_isInProgress)
                {
                    omc.Visible = false;
                    return;
                }

                var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
                if (dte == null)
                {
                    Logging.LogVS("Failed to get DTE service.");
                    omc.Visible = false;
                    return;
                }

                if (!dte.TryGetSelectedProject(out var envProject))
                {
                    omc.Visible = false;
                    return;
                }

                if (_componentModel == null)
                {
                    omc.Visible = false;
                    return;
                }

                var installerServices = _componentModel.GetService<IVsPackageInstallerServices>();
                if (installerServices == null)
                {
                    omc.Visible = false;
                    return;
                }

                var isDpdtInstalled = installerServices.IsPackageInstalled(envProject, "Dpdt.Injector");

                //omc.Visible = true;
                omc.Visible = !isDpdtInstalled;
                return;
            }
            catch (Exception excp)
            {
                Logging.LogVS(excp);
            }
        }

    }
}
