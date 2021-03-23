using DpdtInject.Extension.Helper;
using DpdtInject.Extension.UI.ChainStep;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DpdtInject.Extension.UI.ViewModel.Add.Inner;

namespace DpdtInject.Extension.UI.ViewModel.Add
{
    public class ConstructorArgumentsViewModel : ChainViewModel
    {
        private ICommand? _nextCommand;
        private ICommand? _previousCommand;
        private ICommand? _closeCommand;

        private readonly ChoosedParameters _choosedParameters;
        private readonly Func<Task> _previousStepAction;
        private readonly Func<Task> _nextStepAction;

        public ObservableCollection<ConstructorArgumentViewModel> ConstructorArgumentList
        {
            get;
        } = new ObservableCollection<ConstructorArgumentViewModel>();



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
                            _choosedParameters.ChoosedConstructorArguments = null;
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
                            _choosedParameters.ChoosedConstructorArguments = GetChoosedConstructorArguments();
                            await _nextStepAction();
                        },
                        r => true
                        );
                }

                return _nextCommand;
            }
        }

        public ConstructorArgumentsViewModel(
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
            foreach (var parameter in _choosedParameters.ChoosedConstructor!.Parameters)
            {
                var cavm = new ConstructorArgumentViewModel(
                    parameter
                    );

                if (_choosedParameters.ChoosedConstructorArguments != null)
                {
                    if (_choosedParameters.ChoosedConstructorArguments.Any(cca => ParameterEqualityComparer.Entity.Equals(parameter, cca)))
                    {
                        cavm.IsChecked = true;
                    }

                }

                ConstructorArgumentList.Add(cavm);
            }
        }

        private List<IParameterSymbol> GetChoosedConstructorArguments()
        {
            return ConstructorArgumentList.Where(a => a.IsChecked).Select(a => a.Parameter).ToList();
        }

    }
}
