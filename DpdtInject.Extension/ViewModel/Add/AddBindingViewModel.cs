using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.CodeAnalysis.CSharp;

using Task = System.Threading.Tasks.Task;
using static DpdtInject.Extension.Shared.Logging;

namespace DpdtInject.Extension.ViewModel.Add
{
    public class AddBindingViewModel : BaseViewModel
    {
        private enum StepEnum
        {
            ChooseConstructor,
            ChooseConstructorArgument,
            ChooseBindFrom,
            ChooseTargetMethod,
            ChooseAdditionalParameters
        }


        private readonly CodeLensTarget _target;

        private ContainerAndScanner? _containerAndScanner;
        private INamedTypeSymbol? _targetClass;

        private StepEnum _currentStep = StepEnum.ChooseConstructor;

        private BindScopeEnum _scope = BindScopeEnum.Singleton;

        private Visibility _loadingGridVisibility = Visibility.Visible;
        private Visibility _mainGridVisibility = Visibility.Hidden;

        public Visibility LoadingGridVisibility
        {
            get
            {
                return _loadingGridVisibility;
            }

            set
            {
                _loadingGridVisibility = value;
                OnPropertyChanged(nameof(LoadingGridVisibility));
            }
        }

        public Visibility MainGridVisibility
        {
            get
            {
                return _mainGridVisibility;
            }

            set
            {
                _mainGridVisibility = value;
                OnPropertyChanged(nameof(MainGridVisibility));
            }
        }

        #region constructor list

        public ObservableCollection<ConstructorViewModel> ConstructorList
        {
            get;
        } = new ObservableCollection<ConstructorViewModel>();

        private Visibility _chooseConstructorGridVisibility = Visibility.Hidden;

        public Visibility ChooseConstructorGridVisibility
        {
            get
            {
                return _chooseConstructorGridVisibility;
            }

            set
            {
                _chooseConstructorGridVisibility = value;
                OnPropertyChanged(nameof(ChooseConstructorGridVisibility));
            }
        }

        #endregion

        #region constructor argument list

        public ObservableCollection<ConstructorArgumentViewModel> ConstructorArgumentList
        {
            get;
        } = new ObservableCollection<ConstructorArgumentViewModel>();

        private Visibility _chooseConstructorArgumentGridVisibility = Visibility.Hidden;

        public Visibility ChooseConstructorArgumentGridVisibility
        {
            get
            {
                return _chooseConstructorArgumentGridVisibility;
            }

            set
            {
                _chooseConstructorArgumentGridVisibility = value;
                OnPropertyChanged(nameof(ChooseConstructorArgumentGridVisibility));
            }
        }

        #endregion

        #region choose bind from list

        public ObservableCollection<BindFromViewModel> BindFromList
        {
            get;
        } = new ObservableCollection<BindFromViewModel>();

        private Visibility _chooseBindFromGridVisibility = Visibility.Hidden;

        public Visibility ChooseBindFromGridVisibility
        {
            get
            {
                return _chooseBindFromGridVisibility;
            }

            set
            {
                _chooseBindFromGridVisibility = value;
                OnPropertyChanged(nameof(ChooseBindFromGridVisibility));
            }
        }

        #endregion

        #region target bind method

        public ObservableCollection<TargetMethodViewModel> TargetMethodList
        {
            get;
        } = new ObservableCollection<TargetMethodViewModel>();

        private Visibility _chooseTargetMethodGridVisibility = Visibility.Hidden;

        public Visibility ChooseTargetMethodGridVisibility
        {
            get
            {
                return _chooseTargetMethodGridVisibility;
            }

            set
            {
                _chooseTargetMethodGridVisibility = value;
                OnPropertyChanged(nameof(ChooseTargetMethodGridVisibility));
            }
        }



        #endregion

        #region additional parameters

        private Visibility _chooseAdditionalParametersGridVisibility = Visibility.Hidden;

        public Visibility ChooseAdditionalParametersGridVisibility
        {
            get
            {
                return _chooseAdditionalParametersGridVisibility;
            }

            set
            {
                _chooseAdditionalParametersGridVisibility = value;
                OnPropertyChanged(nameof(ChooseAdditionalParametersGridVisibility));
            }
        }


        public bool IsSingleton
        {
            get
            {
                return _scope == BindScopeEnum.Singleton;
            }

            set
            {
                if (value)
                {
                    _scope = BindScopeEnum.Singleton;
                    OnPropertyChanged(nameof(IsSingleton));
                    OnPropertyChanged(nameof(IsTransient));
                    OnPropertyChanged(nameof(IsCustom));
                    OnPropertyChanged(nameof(IsConstant));
                }
            }
        }

        public bool IsTransient
        {
            get
            {
                return _scope == BindScopeEnum.Transient;
            }

            set
            {
                if (value)
                {
                    _scope = BindScopeEnum.Transient;
                    OnPropertyChanged(nameof(IsSingleton));
                    OnPropertyChanged(nameof(IsTransient));
                    OnPropertyChanged(nameof(IsCustom));
                    OnPropertyChanged(nameof(IsConstant));
                }
            }
        }

        public bool IsCustom
        {
            get
            {
                return _scope == BindScopeEnum.Custom;
            }

            set
            {
                if (value)
                {
                    _scope = BindScopeEnum.Custom;
                    OnPropertyChanged(nameof(IsSingleton));
                    OnPropertyChanged(nameof(IsTransient));
                    OnPropertyChanged(nameof(IsCustom));
                    OnPropertyChanged(nameof(IsConstant));
                }
            }
        }

        public bool IsConstant
        {
            get
            {
                return _scope == BindScopeEnum.Constant;
            }

            set
            {
                if (value)
                {
                    _scope = BindScopeEnum.Constant;
                    OnPropertyChanged(nameof(IsSingleton));
                    OnPropertyChanged(nameof(IsTransient));
                    OnPropertyChanged(nameof(IsCustom));
                    OnPropertyChanged(nameof(IsConstant));
                }
            }
        }

        public bool IsConditionalBinding
        {
            get;
            set;
        }

        #endregion

        private AsyncRelayCommand? _nextCommand;
        private RelayCommand? _previousCommand;
        private RelayCommand? _closeCommand;

        private StepEnum CurrentStep
        {
            get => _currentStep;
            set
            {
                BeforeCloseForm();

                _currentStep = value;

                BeforeOpenForm();

                OnPropertyChanged();
            }
        }

        public string NextButtonTitle
        {
            get
            {
                if (_currentStep == StepEnum.ChooseAdditionalParameters)
                {
                    return "Create";
                }

                return "Next";
            }
        }


        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                {
                    _nextCommand = new AsyncRelayCommand(
                        async (a) => await CreateAsync(a),
                        (a) =>
                        {
                            return IsNextButtonAllowed();
                        });

                }

                return _nextCommand;
            }
        }

        private async Task CreateAsync(object a)
        {
            if (_currentStep == StepEnum.ChooseAdditionalParameters)
            {
                if (a is Window w)
                {
                    try
                    {
                        var newBindingInfo = new NewBindingInfo(
                            GetChoosedBindFroms(),
                            _targetClass!,
                            GetChoosedConstructor(),
                            GetChoosedConstructorArguments(),
                            _scope,
                            IsConditionalBinding
                            );

                        var dm = new DocumentModifier(
                            GetChoosedTargetMethod()
                            );
                        await dm.DoSurgeryAsync(
                            newBindingInfo
                            );

                        w.Close();
                    }
                    catch (Exception excp)
                    {
                        LogVS(excp);
                    }
                }
            }
            else
            {
                CurrentStep = NextButtonProcess();
            }
        }

        public ICommand PreviousCommand
        {
            get
            {
                if (_previousCommand == null)
                {
                    _previousCommand = new RelayCommand(
                        a =>
                        {
                            CurrentStep = PreviousButtonProcess();
                        },
                        (a) =>
                        {
                            return _currentStep != StepEnum.ChooseConstructor;
                        });

                }

                return _previousCommand;
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
                            if (a is Window w)
                            {
                                w.Close();
                            }
                        });
                }

                return _closeCommand;
            }
        }



        /// <inheritdoc />
        public AddBindingViewModel(
            Dispatcher dispatcher,
            CodeLensTarget target
            )
            : base(dispatcher)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            _target = target;
        }


        private StepEnum PreviousButtonProcess()
        {
            switch (_currentStep)
            {
                case StepEnum.ChooseConstructor:
                    throw new ArgumentOutOfRangeException(_currentStep.ToString());
                case StepEnum.ChooseConstructorArgument:
                    return StepEnum.ChooseConstructor;
                case StepEnum.ChooseBindFrom:
                    return StepEnum.ChooseConstructorArgument;
                case StepEnum.ChooseTargetMethod:
                    return StepEnum.ChooseBindFrom;
                case StepEnum.ChooseAdditionalParameters:
                    return StepEnum.ChooseTargetMethod;
                default:
                    throw new ArgumentOutOfRangeException(_currentStep.ToString());
            }
        }

        private StepEnum NextButtonProcess()
        {
            switch (_currentStep)
            {
                case StepEnum.ChooseConstructor:
                    return StepEnum.ChooseConstructorArgument;
                case StepEnum.ChooseConstructorArgument:
                    return StepEnum.ChooseBindFrom;
                case StepEnum.ChooseBindFrom:
                    return StepEnum.ChooseTargetMethod;
                case StepEnum.ChooseTargetMethod:
                    return StepEnum.ChooseAdditionalParameters;
                case StepEnum.ChooseAdditionalParameters:
                    throw new ArgumentOutOfRangeException(_currentStep.ToString());
                default:
                    throw new ArgumentOutOfRangeException(_currentStep.ToString());
            }
        }

        private bool IsNextButtonAllowed()
        {
            switch (_currentStep)
            {
                case StepEnum.ChooseConstructor:
                    if (ConstructorList.Count == 0)
                    {
                        return false;
                    }
                    return true;
                case StepEnum.ChooseConstructorArgument:
                    //if (ConstructorArgumentList.Count(a => a.IsChecked) == 0)
                    //{
                    //    return false;
                    //}
                    return true;
                case StepEnum.ChooseBindFrom:
                    if (BindFromList.Count(a => a.IsChecked) == 0)
                    {
                        return false;
                    }
                    return true;
                case StepEnum.ChooseTargetMethod:
                    if (TargetMethodList.Count == 0)
                    {
                        return false;
                    }
                    return true;
                case StepEnum.ChooseAdditionalParameters:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(_currentStep.ToString());
            }

        }

        private void BeforeCloseForm()
        {
            switch (_currentStep)
            {
                case StepEnum.ChooseConstructor:
                    ChooseConstructorGridVisibility = Visibility.Hidden;
                    break;
                case StepEnum.ChooseConstructorArgument:
                    ChooseConstructorArgumentGridVisibility = Visibility.Hidden;
                    break;
                case StepEnum.ChooseBindFrom:
                    ChooseBindFromGridVisibility = Visibility.Hidden;
                    break;
                case StepEnum.ChooseTargetMethod:
                    ChooseTargetMethodGridVisibility = Visibility.Hidden;
                    break;
                case StepEnum.ChooseAdditionalParameters:
                    ChooseAdditionalParametersGridVisibility = Visibility.Hidden;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(_currentStep.ToString());
            }

        }

        private void BeforeOpenForm()
        {
            switch (_currentStep)
            {
                case StepEnum.ChooseConstructor:
                    ChooseConstructorGridVisibility = Visibility.Visible;
                    break;
                case StepEnum.ChooseConstructorArgument:
                    ConstructorArgumentList.Clear();
                    foreach (var parameter in GetChoosedConstructor().Parameters)
                    {
                        ConstructorArgumentList.Add(
                            new ConstructorArgumentViewModel(
                                _dispatcher,
                                parameter
                                )
                            );
                    }
                    ChooseConstructorArgumentGridVisibility = Visibility.Visible;
                    break;
                case StepEnum.ChooseBindFrom:
                    if (BindFromList.Count > 0)
                    {
                        BindFromList.First(b => b.IsChecked = true);
                        BindFromList.Skip(1).ForEach(b => b.IsChecked = false);
                    }
                    ChooseBindFromGridVisibility = Visibility.Visible;
                    break;
                case StepEnum.ChooseTargetMethod:
                    if (TargetMethodList.Count > 0)
                    {
                        TargetMethodList.First(b => b.IsChecked = true);
                        TargetMethodList.Skip(1).ForEach(b => b.IsChecked = false);
                    }
                    ChooseTargetMethodGridVisibility = Visibility.Visible;
                    break;
                case StepEnum.ChooseAdditionalParameters:
                    ChooseAdditionalParametersGridVisibility = Visibility.Visible;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(_currentStep.ToString());
            }
        }

        private IMethodSymbol GetChoosedConstructor()
        {
            return ConstructorList.First(c => c.IsChecked).Constructor;
        }
        private List<IParameterSymbol> GetChoosedConstructorArguments()
        {
            return ConstructorArgumentList.Where(a => a.IsChecked).Select(a => a.Parameter).ToList();
        }
        private List<INamedTypeSymbol> GetChoosedBindFroms()
        {
            return BindFromList.Where(b => b.IsChecked).Select(b => b.FromType).ToList();
        }
        private MethodBindContainer GetChoosedTargetMethod()
        {
            return TargetMethodList.First(t => t.IsChecked).MethodBindContainer;
        }

        public async Task LoadWindowDataAsync(CancellationTokenSource cts)
        {
            if (cts is null)
            {
                throw new ArgumentNullException(nameof(cts));
            }

            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

            _containerAndScanner = componentModel.GetService<ContainerAndScanner>();


            var targetMethodList = new List<TargetMethodViewModel>();

            var solutionBind = _containerAndScanner.Binds;
            if (solutionBind != null)
            {
                foreach (var ppair in solutionBind.Dict)
                {
                    var projectBind = ppair.Value;

                    foreach (var cpair in projectBind.DictByDisplayString)
                    {
                        var clusterBind = cpair.Value;

                        foreach (var mpair in clusterBind.Dict)
                        {
                            var methodBind = mpair.Value;

                            targetMethodList.Add(
                                new TargetMethodViewModel(
                                    _dispatcher,
                                    methodBind
                                    )

                                );
                        }
                    }
                }
            }
            if (targetMethodList.Count > 0)
            {
                foreach (var bm in targetMethodList.OrderBy(bm => bm.MethodBindContainer.ClusterType.ToDisplayString()))
                {
                    TargetMethodList.Add(bm);
                }

                TargetMethodList[0].IsChecked = true;
            }


            var vsWorkspace = componentModel.GetService<VisualStudioWorkspace>();

            var document = vsWorkspace.GetDocument(
                _target.FilePath,
                _target.ProjectGuid
                );
            if (document == null)
            {
                return;
            }

            var targetClass = await document.GetSymbolAtAsync<INamedTypeSymbol>(
                new Microsoft.CodeAnalysis.Text.TextSpan(
                    _target.TypeSpanStart,
                    _target.TypeSpanLength
                    ),
                cts.Token
                );
            if (targetClass == null)
            {
                return;

            }

            _targetClass = targetClass;

            #region search for all binds from

            var bindFromList = new List<BindFromViewModel>();
            foreach (var inter in targetClass.AllInterfaces)
            {
                bindFromList.Add(
                    new BindFromViewModel(
                        _dispatcher,
                        inter
                        )
                    );
            }
            var baseType = targetClass;
            while (baseType != null)
            {
                bindFromList.Add(
                    new BindFromViewModel(
                        _dispatcher,
                        baseType
                        )
                    );

                baseType = baseType.BaseType;
            }
            if (bindFromList.Count > 0)
            {
                foreach (var bm in bindFromList)
                {
                    BindFromList.Add(bm);
                }

                BindFromList[0].IsChecked = true;
            }

            #endregion

            foreach (var constructor in targetClass.Constructors)
            {
                if (constructor.DeclaredAccessibility.NotIn(
                    Accessibility.Public,
                    Accessibility.Internal,
                    Accessibility.ProtectedAndInternal,
                    Accessibility.ProtectedOrInternal))
                {
                    continue;
                }

                ConstructorList.Add(
                    new ConstructorViewModel(
                        _dispatcher,
                        targetClass,
                        constructor
                        )
                    );
            }
            ConstructorList.First().IsChecked = true;

            OnPropertyChanged();

            LoadingGridVisibility = Visibility.Hidden;
            MainGridVisibility = Visibility.Visible;
            ChooseConstructorGridVisibility = Visibility.Visible;
        }
    }
}
