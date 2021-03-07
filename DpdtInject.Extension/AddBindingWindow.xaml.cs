using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
using EnvDTE80;
using Microsoft.CodeAnalysis.Editing;

using Task = System.Threading.Tasks.Task;
using static DpdtInject.Extension.Shared.Logging;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using DpdtInject.Extension.Options;

namespace DpdtInject.Extension
{
    /// <summary>
    /// Interaction logic for AddBindingWindow.xaml
    /// </summary>
    public partial class AddBindingWindow : DialogWindow
    {
        private readonly CodeLensTarget? _target;
        private CancellationTokenSource? _cts;

        public AddBindingWindow()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public AddBindingWindow(
            CodeLensTarget target
            )
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            _target = target;

            InitializeComponent();

            this.HasMaximizeButton = true;
            this.HasMinimizeButton = false;
        }

        private async void AddBindingWindow_OnLoaded(
            object sender,
            RoutedEventArgs e
            )
        {
            var viewModel = new AddBindingViewModel(
                this.Dispatcher,
                _target!
                );
            DataContext = viewModel;

            _cts = new CancellationTokenSource();
            viewModel.LoadWindowDataAsync(_cts)
                .FileAndForget(nameof(AddBindingWindow_OnLoaded))
                ;
        }

        private void ButtonBase_OnClick(
            object sender,
            RoutedEventArgs e
            )
        {
            MessageBox.Show("Test");
        }

        private void AddBindingWindow_OnKeyUp(
            object sender,
            KeyEventArgs e
            )
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }

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
            return this.ConstructorList.First(c => c.IsChecked).Constructor;
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
            INamedTypeSymbol? baseType = targetClass;
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

    public class TargetMethodViewModel : BaseViewModel
    {
        private bool _isChecked;

        public MethodBindContainer MethodBindContainer
        {
            get;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string VisualRepresentationClassMethod
        {
            get;
        }

        public string VisualRepresentationNamespace
        {
            get;
        }

        /// <inheritdoc />
        public TargetMethodViewModel(
            Dispatcher dispatcher,
            MethodBindContainer mbc
            )
            : base(dispatcher)
        {
            if (mbc is null)
            {
                throw new ArgumentNullException(nameof(mbc));
            }

            MethodBindContainer = mbc;

            VisualRepresentationClassMethod = $"{mbc.ClusterType.Name}.{mbc.MethodSyntax.Identifier.Text}";
            VisualRepresentationNamespace = $"({mbc.ClusterType.ContainingNamespace.ToDisplayString()})";
        }
    }

    public class BindFromViewModel : BaseViewModel
    {
        private bool _isChecked;

        public INamedTypeSymbol FromType
        {
            get;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string VisualRepresentationTypeName
        {
            get;
        }

        public string VisualRepresentationNamespace
        {
            get;
        }


        /// <inheritdoc />
        public BindFromViewModel(
            Dispatcher dispatcher,
            INamedTypeSymbol fromType
            )
            : base(dispatcher)
        {
            if (fromType is null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }

            FromType = fromType;

            VisualRepresentationTypeName = $"{fromType.Name}";
            VisualRepresentationNamespace = $"({fromType.ContainingNamespace.ToDisplayString()})";
        }
    }


    public class ConstructorArgumentViewModel : BaseViewModel
    {
        private bool _isChecked;

        public IParameterSymbol Parameter
        {
            get;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string VisualRepresentationTypeAndName
        {
            get;
        }

        public string VisualRepresentationNamespace
        {
            get;
        }


        /// <inheritdoc />
        public ConstructorArgumentViewModel(
            Dispatcher dispatcher,
            IParameterSymbol parameter
            )
            : base(dispatcher)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            Parameter = parameter;

            VisualRepresentationTypeAndName = $"{parameter.Type.ToDisplayString()} {parameter.Name}";
            VisualRepresentationNamespace = $"{parameter.Type.ContainingNamespace.ToDisplayString()}";
        }
    }


    public class ConstructorViewModel : BaseViewModel
    {
        private bool _isChecked;

        public INamedTypeSymbol TargetType
        {
            get;
        }

        public IMethodSymbol Constructor
        {
            get;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string VisualRepresentation
        {
            get;
        }

        public Brush? ForegroundColor
        {
            get;
        }



        /// <inheritdoc />
        public ConstructorViewModel(
            Dispatcher dispatcher,
            INamedTypeSymbol targetType,
            IMethodSymbol constructor
            )
            : base(dispatcher)
        {
            if (constructor is null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            TargetType = targetType;
            Constructor = constructor;

            var arguments = string.Join(
                ",",
                constructor.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}")
                );

            if (constructor.DeclaredAccessibility == Accessibility.Public)
            {
                VisualRepresentation = $"{targetType.Name}({arguments})";
                ForegroundColor = (Brush)Application.Current.FindResource(VsBrushes.WindowTextKey);
            }
            else
            {
                VisualRepresentation = $"{targetType.Name}({arguments})  ({constructor.DeclaredAccessibility})";
                ForegroundColor = Brushes.Gray;
            }
        }
    }

    public class DocumentModifier
    {
        private readonly MethodBindContainer _targetMethod;

        public DocumentModifier(
            MethodBindContainer targetMethod
            )
        {
            if (targetMethod is null)
            {
                throw new ArgumentNullException(nameof(targetMethod));
            }

            _targetMethod = targetMethod;
        }

        public async Task DoSurgeryAsync(
            NewBindingInfo newBindingInfo
            )
        {
            if (newBindingInfo is null)
            {
                throw new ArgumentNullException(nameof(newBindingInfo));
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2;
            if (dte == null)
            {
                return;
            }

            var componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
            if (componentModel == null)
            {
                return;
            }

            var textManager = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager;
            if (textManager == null)
            {
                return;
            }

            var workspace = (Workspace)componentModel.GetService<VisualStudioWorkspace>();
            if (workspace == null)
            {
                return;
            }

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out var currentActiveView));

            var currentDocumentFilePath = System.IO.Path.Combine(dte.ActiveDocument.Path, dte.ActiveDocument.Name);
            currentActiveView.GetCaretPos(out var currentLine, out var currentColumn);


            var methods = _targetMethod.ClusterType.GetMembers(_targetMethod.MethodSyntax.Identifier.Text);
            if (methods.Length != 1)
            {
                return;
            }

            var method = methods[0];
            if (method.Locations.Length != 1)
            {
                return;
            }

            var methodLocation = method.Locations[0];
            var methodFilePath = methodLocation.SourceTree?.FilePath;

            if (methodFilePath == null)
            {
                return;
            }

            #region open modified document

            var modifiedDocumentHelper = new VisualStudioDocumentHelper(
                methodFilePath
                );

            var lineSpan = methodLocation.GetLineSpan();
            modifiedDocumentHelper.OpenAndNavigate(
                lineSpan.StartLinePosition.Line,
                lineSpan.StartLinePosition.Character,
                lineSpan.EndLinePosition.Line,
                lineSpan.EndLinePosition.Character
                );

            #endregion

            #region switch back to source document

            var sourceDocumentHelper = new VisualStudioDocumentHelper(
                currentDocumentFilePath
                );

            sourceDocumentHelper.OpenAndNavigate(
                currentLine,
                currentColumn,
                currentLine,
                currentColumn
                );

            #endregion

            var document = workspace.GetDocument(
                methodFilePath
                );
            if (document == null)
            {
                return;
            }

            //OptionSet options = workspace.Options;
            //options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, true);
            //options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInProperties, true);


            var surgeon = new SyntaxSurgeon(
                _targetMethod
                );

            var surgedDocument = await surgeon.SurgeAsync(
                document,
                newBindingInfo
                );

            if (surgedDocument == null)
            {
                return;
            }

            workspace.TryApplyChanges(surgedDocument.Project.Solution);
        }
    }
    
    public class SyntaxSurgeon
    {
        private readonly MethodBindContainer _targetMethod;

        public SyntaxSurgeon(
            MethodBindContainer targetMethod
            )
        {
            if (targetMethod is null)
            {
                throw new ArgumentNullException(nameof(targetMethod));
            }

            _targetMethod = targetMethod;
        }

        public async Task<Document?> SurgeAsync(
            Document document,
            NewBindingInfo newBindingInfo
            )
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (newBindingInfo is null)
            {
                throw new ArgumentNullException(nameof(newBindingInfo));
            }

            var documentEditor = await DocumentEditor.CreateAsync(
                document
                );

            #region add new namespaces

            var existingUsings = documentEditor.OriginalRoot
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .ToList()
                ;

            var additionalNamespaces = newBindingInfo.GetUniqueUsings(existingUsings).ToList();
            
            if (existingUsings.Count > 0)
            {
                documentEditor.InsertAfter(existingUsings.Last(), additionalNamespaces);
            }
            else
            {
                documentEditor.InsertAfter(
                    documentEditor.OriginalRoot.DescendantNodes().First(),
                    additionalNamespaces
                    );
            }

            #endregion

            #region add new binding

            var methodSyntax = documentEditor.OriginalRoot
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(m => m.Identifier.Text == _targetMethod.MethodSyntax.Identifier.Text)
                ;
            if (methodSyntax == null)
            {
                return null;
            }
            if (methodSyntax.Body == null)
            {
                return null;
            }

            var leadingTrivia = methodSyntax.Body.GetLeadingTrivia().ToString();


            var bcp = new BindClauseProducer(newBindingInfo);

            var modifiedMethodSyntax = methodSyntax.WithBody(
                methodSyntax.Body.AddStatements(new[] { bcp.ProduceBinding(leadingTrivia) })
                );

            documentEditor.ReplaceNode(
                methodSyntax,
                modifiedMethodSyntax
                );

            #endregion

            var changedDocument = documentEditor.GetChangedDocument();

            var opts = GeneralOptions.Instance;
            if (opts.EnableWhitespaceNormalization)
            {
                var changedRoot = await changedDocument.GetSyntaxRootAsync();

                if (changedRoot == null)
                {
                    return null;
                }

                changedDocument = changedDocument.WithSyntaxRoot(changedRoot.NormalizeWhitespace());
            }

            return changedDocument;
        }
    }

    public class BindClauseProducer
    {
        private readonly NewBindingInfo _newBindingInfo;

        public BindClauseProducer(
            NewBindingInfo newBindingInfo
            )
        {
            if (newBindingInfo is null)
            {
                throw new ArgumentNullException(nameof(newBindingInfo));
            }

            _newBindingInfo = newBindingInfo;
        }

        public StatementSyntax ProduceBinding(
            string leadingTrivia
            )
        {
            var indend1 = leadingTrivia + "    ";
            var indend2 = indend1 + "    ";

            var clauses = new List<string>();

            var bindFroms = string.Join(",", _newBindingInfo.BindFroms.Select(b => b.Name));
            clauses.Add($"Bind<{bindFroms}>()");

            clauses.Add($"{indend2}.To<{_newBindingInfo.BindTo.Name}>()");

            clauses.Add($"{indend2}.With{_newBindingInfo.BindScope}Scope()");

            if (_newBindingInfo.IsConditional)
            {
                clauses.Add($"{indend2}.When(rt => /* compose predicate against rt */ )");
            }

            foreach (var mca in _newBindingInfo.ManualConstructorArguments)
            {
                clauses.Add($"{indend2}.Configure(new ConstructorArgument(\"{mca.Name}\", /* your parameter value */))");
            }

            var clause = string.Join(Environment.NewLine, clauses);

            return
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.ParseExpression(
                        clause
                        )
                    ).WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithLeadingTrivia(SyntaxFactory.Whitespace(indend1))
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                ;

        }
    }

    public class NewBindingInfo
    {
        public IReadOnlyCollection<INamedTypeSymbol> BindFroms
        {
            get;
        }
        public INamedTypeSymbol BindTo
        {
            get;
        }
        public IMethodSymbol Constructor
        {
            get;
        }
        public List<IParameterSymbol> ManualConstructorArguments
        {
            get;
        }
        public BindScopeEnum BindScope
        {
            get;
        }
        public bool IsConditional
        {
            get;
        }

        public NewBindingInfo(
            IReadOnlyCollection<INamedTypeSymbol> bindFroms,
            INamedTypeSymbol bindTo,
            IMethodSymbol constructor,
            List<IParameterSymbol> manualConstructorArguments,
            BindScopeEnum bindScope,
            bool isConditional
            )
        {
            if (bindFroms is null)
            {
                throw new ArgumentNullException(nameof(bindFroms));
            }

            if (bindTo is null)
            {
                throw new ArgumentNullException(nameof(bindTo));
            }

            if (constructor is null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            if (manualConstructorArguments is null)
            {
                throw new ArgumentNullException(nameof(manualConstructorArguments));
            }

            BindFroms = bindFroms;
            BindTo = bindTo;
            Constructor = constructor;
            ManualConstructorArguments = manualConstructorArguments;
            BindScope = bindScope;
            IsConditional = isConditional;
        }

        public IReadOnlyList<UsingDirectiveSyntax> GetNewUsings(
            )
        {
            var result = new Dictionary<string, UsingDirectiveSyntax>();

            foreach (var bindFrom in BindFroms)
            {
                var key = bindFrom.ContainingNamespace.ToDisplayString();
                result[key] = 
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(
                            " " + key
                            )
                        ).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    ;
            }
            {
                var key = BindTo.ContainingNamespace.ToDisplayString();
                result[key] = 
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(
                            " " + key
                            )
                        ).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    ;
            }
            foreach (var constructorArgument in ManualConstructorArguments)
            {
                var key = constructorArgument.Type.ContainingNamespace.ToDisplayString();
                result[key] = 
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(
                            " " + key
                            )
                        ).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    ;
            }

            var constructorArgumentKey = typeof(ConstructorArgument).Namespace;
            result[constructorArgumentKey] =
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.ParseName(
                        " " + constructorArgumentKey
                        )
                    );

            return result.Values.ToList();
        }

        public IEnumerable<UsingDirectiveSyntax> GetUniqueUsings(
            IReadOnlyList<UsingDirectiveSyntax> existingNamespaces
            )
        {
            var newAll = GetNewUsings();
            foreach (var newn in newAll)
            {
                if (existingNamespaces.Any(en => en.Name.ToString() == newn.Name.ToString()))
                {
                    continue;
                }

                yield return newn;
            }
        }

    }
}
