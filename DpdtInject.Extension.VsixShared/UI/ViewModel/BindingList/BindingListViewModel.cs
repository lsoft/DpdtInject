using DpdtInject.Extension.Container;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared;
using DpdtInject.Generator.Core.Binding.Xml;
using DpdtInject.Injector.Src.Bind;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DpdtInject.Extension.UI.ViewModel.BindingList
{
    public class BindingListViewModel : BaseViewModel
    {
        public static Action? RefreshAction;

        private readonly List<BindingItemViewModel> _bindingItemList;
        private string _filterText;
        private string _lowerFilterText;

        private ICommand? _navigateCommand;

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                _lowerFilterText = value.ToLower();

                BindingItemList.Refresh();
                OnPropertyChanged();
            }
        }

        public ICollectionView BindingItemList
        {
            get;
        }

        public ICommand NavigateCommand
        {
            get
            {
                if (_navigateCommand == null)
                {
                    _navigateCommand = new RelayCommand(
                        a =>
                        {
                            var bivm = a as BindingItemViewModel;

                            if (bivm == null)
                            {
                                return;
                            }

                            ThreadHelper.ThrowIfNotOnUIThread(nameof(BindingListViewModel.NavigateCommand));

                            var dh = new VisualStudioDocumentHelper(
                                bivm.BindingStatement.Position.FilePath
                                );

                            dh.OpenAndNavigate(
                                bivm.BindingStatement.Position.StartLine,
                                bivm.BindingStatement.Position.StartColumn,
                                bivm.BindingStatement.Position.EndLine,
                                bivm.BindingStatement.Position.EndColumn
                                );
                        });
                }

                return
                    _navigateCommand;
            }
        }
        public BindingListViewModel()
        {
            _filterText = string.Empty;
            _lowerFilterText = string.Empty;

            _bindingItemList = new List<BindingItemViewModel>();
            ReadAllBindings();

            BindingItemList = CollectionViewSource.GetDefaultView(_bindingItemList);
            BindingItemList.Filter = FilterWrapper;

            RefreshAction = ReadAllBindings;
        }

        private void ReadAllBindings()
        {
            var componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
            if (componentModel == null)
            {
                return;
            }

            var bmc = componentModel.GetService<ContainerAndScanner>();
            var solutionBinds = bmc.Binds;
            if (solutionBinds == null)
            {
                return;
            }

            _bindingItemList.Clear();

            foreach (var cbc in solutionBinds.ClusterBindContainers)
            {
                foreach (var mbc in cbc.MethodBindContainers)
                {
                    foreach (var b in mbc.Bindings)
                    {
                        _bindingItemList.Add(
                            new BindingItemViewModel(
                                b
                                )
                            );
                    }
                }
            }

            OnPropertyChanged();

            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                try
                {
                    BindingItemList.Refresh();
                }
                catch (Exception excp)
                {
                    Logging.LogVS(excp);
                }
            }).FileAndForget(nameof(ReadAllBindings));
        }

        private bool FilterWrapper(
            object w
            )
        {
            try
            {
                var bivm = w as BindingItemViewModel;

                if (bivm == null)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(_lowerFilterText))
                {
                    return true;
                }

                var result = bivm.IsRelatedTo(_lowerFilterText);

                return
                    result;
            }
            catch (Exception excp)
            {
                //ErrorMessage = excp.Message;
                Logging.LogVS(excp);
                //Debug.WriteLine(excp.Message);
                //Debug.Write(excp.StackTrace);
            }

            return false;
        }

    }

    public class BindingItemViewModel : BaseViewModel
    {
        private readonly string _filter;

        public IBindingStatement BindingStatement
        {
            get;
        }

        public string BindingTo
        {
            get;
        }

        public string BindingsFrom
        {
            get;
        }

        public string OtherParameters
        {
            get;
        }
        
        public string BindingLocation
        {
            get;
        }
        public Brush BorderBrush
        {
            get;
        }

        public BindingItemViewModel(
            IBindingStatement bindingStatement
            )
        {
            if (bindingStatement is null)
            {
                throw new ArgumentNullException(nameof(bindingStatement));
            }

            BindingTo = bindingStatement.BindToType.FullDisplayName;
            BindingsFrom = string.Join(", ", bindingStatement.FromTypes.Select(ft => ft.FullDisplayName));
            OtherParameters = bindingStatement.ScopeString + (bindingStatement.IsConditional ? " Conditional" : string.Empty) + (bindingStatement.IsConventional ? " Conventional" : string.Empty);
            BindingLocation = bindingStatement.Position.FilePath + " : " + (bindingStatement.Position.StartLine + 1);

            BorderBrush = ((BindScopeEnum)bindingStatement.ScopeEnumValue) switch
            {
                BindScopeEnum.Singleton => Brushes.Purple,
                BindScopeEnum.Transient => Brushes.Green,
                BindScopeEnum.Constant => Brushes.Blue,
                BindScopeEnum.Custom => Brushes.Gray
            };

            _filter = $"{BindingTo} {BindingsFrom} {OtherParameters} {BindingLocation}".ToLower();
            BindingStatement = bindingStatement;
        }

        internal bool IsRelatedTo(string lowerFilterText)
        {
            return _filter.Contains(lowerFilterText);
        }
    }
}
