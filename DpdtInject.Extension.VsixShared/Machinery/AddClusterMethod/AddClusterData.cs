using DpdtInject.Extension.Helper;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;
using DpdtInject.Injector.Src;

namespace DpdtInject.Extension.Machinery.AddClusterMethod
{
    public class AddClusterData
    {
        private readonly Workspace _workspace;

        public EnvDTE.Project Project
        {
            get;
        }
        
        public ProjectItem? ProjectItem
        {
            get;
        }

        public string AdditionalFolders
        {
            get;
            set;
        }

        public ObservableCollection<string> ClusterClassNameList
        {
            get;
        }

        public string ClusterClassName
        {
            get;
            set;
        }

        public string BindingMethodName
        {
            get;
            set;
        }

        public AddClusterData(
            EnvDTE.Project project,
            ProjectItem? projectItem,
            Workspace workspace,
            string additionalFolders
            )
        {
            if (project is null)
            {
                throw new System.ArgumentNullException(nameof(project));
            }

            if (workspace is null)
            {
                throw new ArgumentNullException(nameof(workspace));
            }

            //projectItem allowed to be null

            if (additionalFolders is null)
            {
                throw new System.ArgumentNullException(nameof(additionalFolders));
            }

            Project = project;
            ProjectItem = projectItem;
            _workspace = workspace;
            AdditionalFolders = additionalFolders;
            ClusterClassNameList = new ObservableCollection<string>();

            BindingMethodName = "DeclareBindings";
            ClusterClassName = ClusterClassNameList.FirstOrDefault() ?? "MyCluster";
        }

        public async Task<string?> ProcessAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2;
            if (dte == null)
            {
                return null;
            }

            var currentProjectItems = (this.ProjectItem?.ProjectItems ?? this.Project.ProjectItems);
            foreach (var folder in SplitAdditionalFolders())
            {
                var existedFolder = (
                    from ProjectItem child in currentProjectItems
                    where child?.Kind == SolutionHelper.ProjectItemKindFolder
                    where StringComparer.CurrentCultureIgnoreCase.Compare(child.Name, folder) == 0
                    select child).FirstOrDefault();

                if (existedFolder != null)
                {
                    currentProjectItems = existedFolder.ProjectItems;
                }
                else
                {
                    currentProjectItems = currentProjectItems.AddFolder(folder).ProjectItems;
                }
            }

            var fileName = $"{this.ClusterClassName}.cs";
            var filePath = Path.Combine(
                (currentProjectItems.Parent as dynamic)!.Properties.Item("FullPath").Value.ToString(),
                fileName
                );

            if (File.Exists(filePath))
            {
                return null;
            }

            var namesp = GetDestinationNamespace();

            var attrName = typeof(DpdtBindingMethodAttribute).Name;
            var cutAttrName = attrName.Substring(0, attrName.Length - "Attribute".Length);

            File.WriteAllText(
                filePath,
                @$"using System;
using {typeof(DefaultCluster).Namespace};

namespace {namesp}
{{
    public partial class {this.ClusterClassName} : DefaultCluster
    {{
        public {this.ClusterClassName}()
            : this((ICluster)null!) //to remove this line compilation error, just remove this line or (better) add at least 1 binding
        {{
        }}

        [{cutAttrName}]
        public void {this.BindingMethodName}()
        {{
            //TODO: add bindings here
        }}
    }}
}}
");

            currentProjectItems.AddFromFile(filePath);

            return filePath;
        }

        public bool TryValidate(out string? errorMessage)
        {
            if (AdditionalFolders.Contains(Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar))
            {
                errorMessage = "Additional folder path is invalid";
                return false;
            }
            if (AdditionalFolders.Contains(Path.AltDirectorySeparatorChar.ToString() + Path.AltDirectorySeparatorChar))
            {
                errorMessage = "Additional folder path is invalid";
                return false;
            }


            foreach (var folder in SplitAdditionalFolders())
            {
                if (folder.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
                {
                    errorMessage = "Additional folder path is invalid";
                    return false;
                }
            }

            if (string.IsNullOrEmpty(ClusterClassName))
            {
                errorMessage = "Cluster name should be set";
                return false;
            }
            if (!char.IsLetter(ClusterClassName![0]) && ClusterClassName![0] != '_')
            {
                errorMessage = "Cluster name should start with a letter or _";
                return false;
            }
            if (ClusterClassName.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
            {
                errorMessage = "Cluster name is invalid";
                return false;
            }

            if (string.IsNullOrEmpty(BindingMethodName))
            {
                errorMessage = "Binding method name should be set";
                return false;
            }
            if (!char.IsLetter(BindingMethodName[0]) && BindingMethodName![0] != '_')
            {
                errorMessage = "Binding method name should start with a letter or _";
                return false;
            }
            if (BindingMethodName.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
            {
                errorMessage = "Binding method name is invalid";
                return false;
            }

            //check against target file exists
            var targetFilePath = GetTargetFilePath();
            if (File.Exists(targetFilePath))
            {
                errorMessage = $"File already exists: {targetFilePath}";
                return false;
            }

            //check against target class\method exists

            var destinationNamespace = GetDestinationNamespace();
            var fullClassName = $"{destinationNamespace}.{ClusterClassName}";

            ISymbol? foundTargetMethod = null;
            ThreadHelper.JoinableTaskFactory.Run(
                async () =>
                {
                    var typeDict = await _workspace.GetAllTypesInNamespaceAsync(destinationNamespace);
                    if (typeDict.TryGetValue(fullClassName, out var foundTargetClass))
                    {
                        //this type already exists
                        //check for method exists
                        foundTargetMethod = (
                            from member in foundTargetClass.GetMembers()
                            where member.Kind == SymbolKind.Method
                            where member.Name == BindingMethodName
                            select member
                            ).FirstOrDefault();
                    }
                });

            if (foundTargetMethod != null)
            {
                //class and method already exists
                errorMessage = $"{fullClassName} with the method {BindingMethodName} already exists";
                return false;
            }

            errorMessage = null;
            return true;
        }


        public async System.Threading.Tasks.Task UpdateClusterClassNameListAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            ClusterClassNameList.Clear();

            var destinationNamespace = GetDestinationNamespace();

            var typeDict = await _workspace.GetAllTypesInNamespaceAsync(destinationNamespace);
            foreach (var pair in typeDict)
            {
                var t = pair.Value;
                if (t.BaseType!.ToDisplayString() == typeof(DefaultCluster).FullName)
                {
                    ClusterClassNameList.Add(t.Name);
                }
            }
        }


        private string[] SplitAdditionalFolders()
        {
            return AdditionalFolders.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string GetDestinationNamespace()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var namesp =
                this.ProjectItem != null
                    ? this.ProjectItem.Properties.Item("DefaultNamespace").Value.ToString()
                    : Project.Properties.Item("RootNamespace").Value.ToString()
                    ;

            if (!string.IsNullOrEmpty(AdditionalFolders))
            {
                namesp += "." + string.Join(".", SplitAdditionalFolders());
            }

            return namesp;
        }

        private string GetTargetFilePath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var selectedItemPath = (this.ProjectItem?.Properties.Item("FullPath").Value.ToString() ?? this.Project.Properties.Item("FullPath").Value.ToString());

            var fileName = $"{this.ClusterClassName}.cs";
            
            var filePath = Path.Combine(
                selectedItemPath,
                AdditionalFolders,
                fileName
                );

            return filePath;
        }
    }
}
