using EnvDTE;
using EnvDTE80;
using System;

namespace DpdtInject.Extension.Helper
{
    public static class DTEHelper
    {
        public static bool TryGetSelectedProject(
            this DTE2 dte,
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

    }
}
