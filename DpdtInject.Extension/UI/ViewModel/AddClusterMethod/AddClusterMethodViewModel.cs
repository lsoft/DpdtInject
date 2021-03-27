using DpdtInject.Extension.Helper;
using DpdtInject.Injector;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension.UI.ViewModel.AddClusterMethod
{
    public class AddClusterMethodViewModel : BaseViewModel
    {
        private readonly AddClusterData _addClusterData;

        private ICommand? _closeCommand;
        private ICommand? _nextCommand;

        public string AdditionalFolders
        {
            get => _addClusterData.AdditionalFolders;
            set
            {
                _addClusterData.AdditionalFolders = value;
                OnPropertyChanged();
            }
        }

        public string? ClusterClassName
        {
            get => _addClusterData.ClusterClassName;
            set
            {
                _addClusterData.ClusterClassName = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ClusterClassNameList
        {
            get;
        }

        public string BindingMethodName
        {
            get => _addClusterData.BindingMethodName;
            set
            {
                _addClusterData.BindingMethodName = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get
            {
                _ = _addClusterData.TryValidate(out var errorMessage);
                return errorMessage ?? string.Empty;
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(
                        a =>
                        {
                            if (a is System.Windows.Window w)
                            {
                                w.Close();
                            }
                        });
                }

                return _closeCommand;
            }
        }

        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                {
                    _nextCommand = new AsyncRelayCommand(
                        async a =>
                        {
                            var filePath = await _addClusterData.ProcessAsync();

                            if (filePath == null)
                            {
                                return;
                            }

                            if (a is System.Windows.Window w)
                            {
                                w.Close();
                            }

                            ThreadHelper.JoinableTaskFactory.RunAsync(
                                () => Task.Run(
                                    () =>
                                    {
                                        Task.Delay(500);

                                        var dh = new VisualStudioDocumentHelper(filePath);
                                        dh.Open();
                                    })).FileAndForget(nameof(VisualStudioDocumentHelper));

                        },
                        r => _addClusterData.TryValidate(out _)
                        );
                }

                return _nextCommand;
            }
        }

        public AddClusterMethodViewModel(
            AddClusterData addClusterData
            )
        {
            if (addClusterData is null)
            {
                throw new System.ArgumentNullException(nameof(addClusterData));
            }

            _addClusterData = addClusterData;

            ClusterClassNameList = new ObservableCollection<string>(_addClusterData.ClusterClassNameList);
        }
    }

    public class AddClusterData
    {
        public Project Project
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

        public List<string> ClusterClassNameList
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
            Project project,
            ProjectItem? projectItem,
            string additionalFolders,
            List<string> clusterClassNameList
            )
        {
            if (project is null)
            {
                throw new System.ArgumentNullException(nameof(project));
            }

            //projectItem allowed to be null

            if (additionalFolders is null)
            {
                throw new System.ArgumentNullException(nameof(additionalFolders));
            }

            if (clusterClassNameList is null)
            {
                throw new System.ArgumentNullException(nameof(clusterClassNameList));
            }

            Project = project;
            ProjectItem = projectItem;
            AdditionalFolders = additionalFolders;
            ClusterClassNameList = clusterClassNameList;

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
                (currentProjectItems.Parent as ProjectItem)!.Properties.Item("FullPath").Value.ToString(),
                fileName
                );

            var namesp =
                this.ProjectItem != null
                    ? this.ProjectItem.Properties.Item("DefaultNamespace").Value.ToString() + "." + AdditionalFolders.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.')
                    : Project.Properties.Item("RootNamespace").Value.ToString() + "." + AdditionalFolders.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.')
                    ;

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
            : this((ICluster)null!) //remove this line or better add at least 1 binding to remove compilation error
        {{
        }}

        [{cutAttrName}]
        public void {this.BindingMethodName}()
        {{
            //TODO: add bingins here
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

            errorMessage = null;
            return true;
        }


        private string[] SplitAdditionalFolders()
        {
            return AdditionalFolders.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
        }
    }
}
