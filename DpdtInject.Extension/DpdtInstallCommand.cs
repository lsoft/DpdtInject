﻿using DpdtInject.Extension.Options;
using DpdtInject.Extension.Shared;
using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension
{
    public class DpdtInstallCommand
    {
        public static string ProjectKind = "{52AEFF70-BBD8-11d2-8598-006097C68E81}";


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
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="DpdtInstallCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private DpdtInstallCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
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
                return package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

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
        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                var componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
                if (componentModel == null)
                {
                    return;
                }

                var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
                if (dte == null)
                {
                    Logging.LogVS("Failed to get DTE service.");
                    return;
                }

                if (!TryGetSelectedProject(dte, out var envProject))
                {
                    return;
                }
                
                var packageInstaller = componentModel.GetService<IVsPackageInstaller2>();
                packageInstaller.InstallLatestPackage(null, envProject, "Dpdt.Injector", true, false);
            }
            catch (Exception excp)
            {
                Logging.LogVS(excp);
                ShowError(excp.Message + Environment.NewLine + excp.StackTrace);
            }
        }


        private void ShowError(string errorMessage)
        {
            VsShellUtilities.ShowMessageBox(
                package,
                errorMessage,
                $"Error has been found",
                OLEMSGICON.OLEMSGICON_WARNING,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
        }

        private static bool TryGetSelectedProject(
            DTE2 dte,
            out EnvDTE.Project? envProject
            )
        {
            var uih = dte.ToolWindows.SolutionExplorer;
            var selectedItems = (Array)uih.SelectedItems;
            if (selectedItems.Length != 1)
            {
                envProject = null;
                return false;
            }

            foreach (UIHierarchyItem selectedItem in selectedItems)
            {
                if (!(selectedItem.Object is EnvDTE.Project project))
                {
                    envProject = null;
                    return false;
                }

                envProject = project;
                return true;
            }

            envProject = null;
            return false;
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

                var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
                if (dte == null)
                {
                    Logging.LogVS("Failed to get DTE service.");
                    omc.Visible = false;
                    return;
                }

                if (!TryGetSelectedProject(dte, out var envProject))
                {
                    omc.Visible = false;
                    return;
                }

                var componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
                if (componentModel == null)
                {
                    omc.Visible = false;
                    return;
                }

                var installerServices = componentModel.GetService<IVsPackageInstallerServices>();
                if (installerServices == null)
                {
                    omc.Visible = false;
                    return;
                }

                var isDpdtInstalled = installerServices.IsPackageInstalled(envProject, "Dpdt.Injector");

                omc.Visible = !isDpdtInstalled;
                //omc.Visible = true;
                return;
            }
            catch (Exception excp)
            {
                Logging.LogVS(excp);
            }
        }

    }
}
