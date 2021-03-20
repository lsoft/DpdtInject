using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;
using DpdtInject.Extension.UI.ChainStep;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                            _choosedParameters.ChoosedConstructor = GetChoosedConstructor();
                            await _nextStepAction();
                        },
                        r => true
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

        private IMethodSymbol GetChoosedConstructor()
        {
            return ConstructorList.First(c => c.IsChecked).Constructor;
        }

    }
}
