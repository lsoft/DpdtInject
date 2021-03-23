using DpdtInject.Extension.Helper;
using DpdtInject.Extension.UI.ChainStep;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;
using DpdtInject.Extension.UI.ViewModel.Add.Inner;

namespace DpdtInject.Extension.UI.ViewModel.Add
{
    public class ConstructorListViewModel : ChainViewModel
    {
        private readonly Func<Task> _nextStepAction;
        private readonly ChoosedParameters _choosedParameters;

        private ICommand? _nextCommand;
        private ICommand? _closeCommand;

        public ObservableCollection<ConstructorViewModel> ConstructorList
        {
            get;
        } = new ObservableCollection<ConstructorViewModel>();


        public string ErrorMessage
        {
            get;
            set;
        } = string.Empty;

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

        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                {
                    _nextCommand = new AsyncRelayCommand(
                        async a =>
                        {
                            if (TryGetChoosedConstructor(out var choosedConstructor))
                            {
                                _choosedParameters.ChoosedConstructor = choosedConstructor;
                                await _nextStepAction();
                            }
                        },
                        r => TryGetChoosedConstructor(out _)
                        );
                }

                return _nextCommand;
            }
        }

        public ConstructorListViewModel(
            ChoosedParameters choosedParameters,
            Func<Task> nextStepAction
            )
        {
            if (choosedParameters is null)
            {
                throw new ArgumentNullException(nameof(choosedParameters));
            }
            if (nextStepAction is null)
            {
                throw new ArgumentNullException(nameof(nextStepAction));
            }

            _choosedParameters = choosedParameters;
            _nextStepAction = nextStepAction;
        }

        public override async Task StartAsync()
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

            var vsWorkspace = componentModel.GetService<VisualStudioWorkspace>();

            var document = vsWorkspace.GetDocument(
                _choosedParameters.Target.FilePath,
                _choosedParameters.Target.ProjectGuid
                );
            if (document == null)
            {
                ErrorMessage = "Cannot obtain a document";
                return;
            }

            var targetClass = await document.GetSymbolAtAsync<INamedTypeSymbol>(
                new Microsoft.CodeAnalysis.Text.TextSpan(
                    _choosedParameters.Target.TypeSpanStart,
                    _choosedParameters.Target.TypeSpanLength
                    ),
                CancellationToken.None
                );
            if (targetClass == null)
            {
                ErrorMessage = "Cannot obtain a symbol info";
                return;
            }


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
                        targetClass,
                        constructor
                        )
                    );
            }

            if (ConstructorList.Count == 0)
            {
                ErrorMessage = "No constructors available";
                return;
            }

            if (_choosedParameters.ChoosedConstructor != null)
            {
                var selected = ConstructorList.FirstOrDefault(c => SymbolEqualityComparer.Default.Equals(c.Constructor, _choosedParameters.ChoosedConstructor));
                if (selected != null)
                {
                    selected.IsChecked = true;
                    return;
                }
            }

            ConstructorList.First().IsChecked = true;
        }

        private bool TryGetChoosedConstructor(out IMethodSymbol? choosedConstructor)
        {
            var choosedConstructorVM = ConstructorList.FirstOrDefault(c => c.IsChecked);
            if (choosedConstructorVM == null)
            {
                choosedConstructor = null;
                return false;
            }

            choosedConstructor = choosedConstructorVM.Constructor;
            return true;
        }

    }
}
