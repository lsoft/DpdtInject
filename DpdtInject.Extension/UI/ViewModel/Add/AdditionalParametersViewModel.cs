using DpdtInject.Extension.UI.ChainStep;
using DpdtInject.Injector.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DpdtInject.Extension.UI.ViewModel.Add
{
    public class AdditionalParametersViewModel : ChainViewModel
    {
        private readonly ChoosedParameters _choosedParameters;
        private readonly Func<Task> _previousStepAction;
        private readonly Func<Task> _nextStepAction;

        private ICommand? _nextCommand;
        private ICommand? _previousCommand;
        private ICommand? _closeCommand;

        private BindScopeEnum _scope = BindScopeEnum.Singleton;


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


        public ICommand PreviousCommand
        {
            get
            {
                if (_previousCommand == null)
                {
                    _previousCommand = new AsyncRelayCommand(
                        async a =>
                        {
                            _choosedParameters.Scope = BindScopeEnum.Singleton;
                            _choosedParameters.IsConditionalBinding = false;

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
                            _choosedParameters.Scope = _scope;
                            _choosedParameters.IsConditionalBinding = IsConditionalBinding;

                            await _nextStepAction();
                        },
                        r => true
                        );
                }

                return _nextCommand;
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

        public AdditionalParametersViewModel(
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
            _scope = _choosedParameters.Scope;
            IsConditionalBinding = _choosedParameters.IsConditionalBinding;
        }
    }
}
