using DpdtInject.Extension.Container;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared;
using DpdtInject.Generator.Core.Binding.Xml;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Helper;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DpdtInject.Extension.UI.ViewModel.BindingList
{
    public class BindingListViewModel : BaseViewModel
    {
        public static Action? RefreshAction;

        private readonly List<BindingItemViewModel> _bindingItemList;
        private FilterValue? _filterValue;
        private string _filterText;

        private int _filteredCount;

        private ICommand? _filterConstructorArgumentCommand;
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
                _filteredCount = 0;
                _filterText = value;
                _filterValue = new FilterValue(value);

                BindingItemList.Refresh();
                OnPropertyChanged();
            }
        }

        public ICollectionView BindingItemList
        {
            get;
        }

        public ICommand FilterConstructorArgumentCommand
        {
            get
            {
                if (_filterConstructorArgumentCommand == null)
                {
                    _filterConstructorArgumentCommand = new RelayCommand(
                        a =>
                        {
                            var type = a as string;

                            if (string.IsNullOrEmpty(type))
                            {
                                return;
                            }

                            FilterText = nameof(FilterSearchEnum.CA) + ":" + type!;
                        });
                }

                return
                    _filterConstructorArgumentCommand;
            }
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
            _filterValue = null;

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

                if (_filterValue == null)
                {
                    _filteredCount++;
                    return true;
                }
                if (string.IsNullOrEmpty(_filterValue.Value))
                {
                    _filteredCount++;
                    return true;
                }

                if (!bivm.IsRelatedTo(_filterValue))
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

    public class FilterValue
    {
        public string Value
        {
            get;
        }

        public FilterSearchEnum FilterSearch
        {
            get;
        }

        public FilterValue(
            string filterText
            )
        {
            if (filterText is null)
            {
                throw new ArgumentNullException(nameof(filterText));
            }

            Value = filterText;

            if (Value.Contains(":"))
            {
                var parts = Value.Split(':');
                if (parts.Length != 2)
                {
                    FilterSearch = FilterSearchEnum.All;
                }

                switch (true)
                {
                    case bool when parts[0].Equals(nameof(FilterSearchEnum.From), StringComparison.InvariantCultureIgnoreCase):
                        FilterSearch = FilterSearchEnum.From;
                        break;
                    case bool when parts[0].Equals(nameof(FilterSearchEnum.To), StringComparison.InvariantCultureIgnoreCase):
                        FilterSearch = FilterSearchEnum.To;
                        break;
                    case bool when parts[0].Equals(nameof(FilterSearchEnum.CA), StringComparison.InvariantCultureIgnoreCase):
                        FilterSearch = FilterSearchEnum.CA;
                        break;
                    case bool when parts[0].Equals(nameof(FilterSearchEnum.OtherParameters), StringComparison.InvariantCultureIgnoreCase):
                        FilterSearch = FilterSearchEnum.OtherParameters;
                        break;
                    case bool when parts[0].Equals(nameof(FilterSearchEnum.Other), StringComparison.InvariantCultureIgnoreCase):
                        FilterSearch = FilterSearchEnum.Other;
                        break;
                    case bool when parts[0].Equals(nameof(FilterSearchEnum.BindingLocation), StringComparison.InvariantCultureIgnoreCase):
                        FilterSearch = FilterSearchEnum.BindingLocation;
                        break;
                    case bool when parts[0].Equals(nameof(FilterSearchEnum.Location), StringComparison.InvariantCultureIgnoreCase):
                        FilterSearch = FilterSearchEnum.Location;
                        break;
                    default:
                        FilterSearch = FilterSearchEnum.All;
                        break;
                }

                Value = parts[1];
            }
            else
            {
                FilterSearch = FilterSearchEnum.All;
            }
        }

    }

    /// <summary>
    /// Where to search this filter
    /// </summary>
    [Flags]
    public enum FilterSearchEnum
    {
        /// <summary>
        /// Bind from types
        /// </summary>
        From = 1,

        /// <summary>
        /// Bind to type
        /// </summary>
        To = 2,

        /// <summary>
        /// Constructor arguments
        /// </summary>
        CA = 4,

        /// <summary>
        /// Other parameters
        /// </summary>
        OtherParameters = 8,

        /// <summary>
        /// Other parameters
        /// </summary>
        Other = 8,

        /// <summary>
        /// Binding location
        /// </summary>
        BindingLocation = 16,

        /// <summary>
        /// Binding location
        /// </summary>
        Location = 16,

        /// <summary>
        /// Everywhere
        /// </summary>
        All = From | To | CA | OtherParameters | BindingLocation
    }

    public class BindingItemViewModel : BaseViewModel
    {

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

        public Visibility ConstructorArgumentsVisibility => ConstructorArguments.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        public List<string> ConstructorArguments
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
            ConstructorArguments = bindingStatement.ConstructorArguments?.FullDisplayNames ?? new List<string>();

            BorderBrush = (BindScopeEnum)bindingStatement.ScopeEnumValue switch
            {
                BindScopeEnum.Singleton => Brushes.Purple,
                BindScopeEnum.Transient => Brushes.Green,
                BindScopeEnum.Constant => Brushes.Blue,
                BindScopeEnum.Custom => Brushes.Gray,
                _ => Brushes.Red
            };

            BindingStatement = bindingStatement;
        }

        internal bool IsRelatedTo(FilterValue fv)
        {
            if (fv is null)
            {
                throw new ArgumentNullException(nameof(fv));
            }

            if (fv.FilterSearch == FilterSearchEnum.All || fv.FilterSearch == FilterSearchEnum.From)
            {
                if (BindingsFrom.Contains(fv.Value))
                {
                    return true;
                }
            }
            if (fv.FilterSearch == FilterSearchEnum.All || fv.FilterSearch == FilterSearchEnum.To)
            {
                if (BindingTo.Contains(fv.Value))
                {
                    return true;
                }
            }
            if (fv.FilterSearch == FilterSearchEnum.All || fv.FilterSearch == FilterSearchEnum.CA)
            {
                if (ConstructorArguments.Any(ca => ca.Contains(fv.Value)))
                {
                    return true;
                }
            }
            if (fv.FilterSearch == FilterSearchEnum.All || fv.FilterSearch.In(FilterSearchEnum.Other, FilterSearchEnum.OtherParameters))
            {
                if (OtherParameters.Contains(fv.Value))
                {
                    return true;
                }
            }
            if (fv.FilterSearch == FilterSearchEnum.All || fv.FilterSearch.In(FilterSearchEnum.BindingLocation, FilterSearchEnum.Location))
            {
                if (BindingLocation.Contains(fv.Value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
