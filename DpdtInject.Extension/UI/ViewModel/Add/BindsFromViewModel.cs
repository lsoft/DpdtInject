using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;
using DpdtInject.Extension.UI.ChainStep;
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
    public class BindsFromViewModel : ChainViewModel
    {
        private readonly ChoosedParameters _choosedParameters;
        private readonly Func<Task> _previousStepAction;
        private readonly Func<Task> _nextStepAction;

        private ICommand? _nextCommand;
        private ICommand? _previousCommand;
        private ICommand? _closeCommand;

        private ICommand? _invertStatusCommand;

        private INamedTypeSymbol? _targetClass;

        public ObservableCollection<BindFromViewModel> BindFromList
        {
            get;
        } = new ObservableCollection<BindFromViewModel>();

        public ICommand InvertStatusCommand
        {
            get
            {
                if (_invertStatusCommand == null)
                {
                    _invertStatusCommand = new RelayCommand(
                        a =>
                        {
                            var selected = BindFromList.Where(i => i.IsSelected).ToList();

                            if (selected.Count == 0)
                            {
                                return;
                            }

                            var newValue = !selected[0].IsChecked;
                            selected.ForEach(s => s.IsChecked = newValue);
                        }
                        );
                }

                return _invertStatusCommand;
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

        public ICommand PreviousCommand
        {
            get
            {
                if (_previousCommand == null)
                {
                    _previousCommand = new AsyncRelayCommand(
                        async a =>
                        {
                            _choosedParameters.TargetClass = null;
                            _choosedParameters.ChoosedBindsFrom = null;
                            await _previousStepAction();
                        },
                        r => true
                        );
                }

                return _previousCommand;
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
                            _choosedParameters.TargetClass = _targetClass;
                            _choosedParameters.ChoosedBindsFrom = GetChoosedBindFroms();
                            await _nextStepAction();
                        },
                        r => true
                        );
                }

                return _nextCommand;
            }
        }

        public BindsFromViewModel(
            ChoosedParameters choosedParameters,
            Func<Task> previousStepAction,
            Func<Task> nextStepAction
            )
        {
            if (choosedParameters is null)
            {
                throw new ArgumentNullException(nameof(choosedParameters));
            }

            if (previousStepAction is null)
            {
                throw new ArgumentNullException(nameof(previousStepAction));
            }

            if (nextStepAction is null)
            {
                throw new ArgumentNullException(nameof(nextStepAction));
            }
            _choosedParameters = choosedParameters;
            _previousStepAction = previousStepAction;
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

            _targetClass = targetClass;

            var bindFromList = new List<BindFromViewModel>();

            foreach (var (level, parent) in targetClass.IterateInterfaces())
            {
                bindFromList.Add(
                    new BindFromViewModel(
                        level,
                        parent
                        )
                    );
            }
            foreach (var (level, parent) in targetClass.IterateClasses())
            {
                bindFromList.Add(
                    new BindFromViewModel(
                        level,
                        parent
                        )
                    );
            }

            if (bindFromList.Count == 0)
            {
                return;
            }

            foreach (var bm in bindFromList)
            {
                BindFromList.Add(bm);
            }

            if (_choosedParameters.ChoosedBindsFrom != null && _choosedParameters.ChoosedBindsFrom.Count != 0)
            {
                foreach (var bm in BindFromList)
                {
                    if (_choosedParameters.ChoosedBindsFrom.Any(cbf => SymbolEqualityComparer.Default.Equals(cbf, bm.FromType)))
                    {
                        bm.IsChecked = true;
                    }
                }
            }
            else
            {
                BindFromList[0].IsChecked = true;
            }
        }

        private List<INamedTypeSymbol> GetChoosedBindFroms()
        {
            return BindFromList.Where(b => b.IsChecked).Select(b => b.FromType).ToList();
        }

    }
}
