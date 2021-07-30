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

        private int _filteredCount;

        private ICommand? _navigateCommand;
        private bool _includeSingleton = true;
        private bool _includeTransient = true;
        private bool _includeConstant = true;
        private bool _includeCustom = true;
        private bool? _includeConditional = null;
        private bool? _includeConventional = null;

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

        public bool IncludeSingleton
        {
            get => _includeSingleton;
            set
            {
                _includeSingleton = value;

                RefreshBindingList();
            }
        }

        public bool IncludeTransient
        {
            get => _includeTransient;
            set
            {
                _includeTransient = value;

                RefreshBindingList();
            }
        }

        public bool IncludeConstant
        {
            get => _includeConstant;
            set
            {
                _includeConstant = value;

                RefreshBindingList();
            }
        }

        public bool IncludeCustom
        {
            get => _includeCustom;
            set
            {
                _includeCustom = value;

                RefreshBindingList();
            }
        }

        public bool? IncludeConditional
        {
            get => _includeConditional;
            set
            {
                _includeConditional = value;

                RefreshBindingList();
            }
        }

        public bool? IncludeConventional
        {
            get => _includeConventional;
            set
            {
                _includeConventional = value;

                RefreshBindingList();
            }
        }

        public string SummaryText
        {
            get
            {
                return $"Total binding found: {_bindingItemList.Count}, showed: {_filteredCount}.";
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
                    RefreshBindingList();
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

                if (bivm.BindingStatement.ScopeEnumValue == (int)BindScopeEnum.Singleton)
                {
                    if (!_includeSingleton)
                    {
                        return false;
                    }
                }
                if (bivm.BindingStatement.ScopeEnumValue == (int)BindScopeEnum.Transient)
                {
                    if (!_includeTransient)
                    {
                        return false;
                    }
                }
                if (bivm.BindingStatement.ScopeEnumValue == (int)BindScopeEnum.Constant)
                {
                    if (!_includeConstant)
                    {
                        return false;
                    }
                }
                if (bivm.BindingStatement.ScopeEnumValue == (int)BindScopeEnum.Custom)
                {
                    if (!_includeCustom)
                    {
                        return false;
                    }
                }
                if (_includeConditional.HasValue)
                {
                    if (bivm.BindingStatement.IsConditional != _includeConditional.Value)
                    {
                        return false;
                    }
                }
                if (_includeConventional.HasValue)
                {
                    if (bivm.BindingStatement.IsConventional != _includeConventional.Value)
                    {
                        return false;
                    }
                }

                if (string.IsNullOrEmpty(_lowerFilterText))
                {
                    _filteredCount++;
                    return true;
                }

                if (!bivm.IsRelatedTo(_lowerFilterText))
                {
                    return false;
                }

                _filteredCount++;
                return true;
            }
            catch (Exception excp)
            {
                Logging.LogVS(excp);
            }

            return false;
        }

        private void RefreshBindingList()
        {
            _filteredCount = 0;
            BindingItemList.Refresh();
            OnPropertyChanged();
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
