using DpdtInject.Extension.Shared;
using DpdtInject.Extension.UI.Control;
using DpdtInject.Extension.UI.ViewModel.Add;
using System;
using System.Windows.Controls;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension.UI.ChainStep
{
    public class BindsFromChainStep : MedianeChainStep, IChainStep
    {
        private readonly ContentControl _targetControl;
        private readonly ChoosedParameters _choosedParameters;

        private IChainStep? _previousStepIfConstructorArgumentExists;
        private IChainStep? _previousStepIfNoConstructorArgumentExists;
        private IChainStep? _nextStep;


        public BindsFromChainStep(
            ContentControl targetControl,
            ChoosedParameters choosedParameters
            )
        {
            if (targetControl is null)
            {
                throw new ArgumentNullException(nameof(targetControl));
            }

            if (choosedParameters is null)
            {
                throw new ArgumentNullException(nameof(choosedParameters));
            }

            _targetControl = targetControl;
            _choosedParameters = choosedParameters;
        }


        public async Task CreateAsync()
        {
            var v = new BindsFromControl();

            var vm = new BindsFromViewModel(
                _choosedParameters,
                PreviousAsync,
                NextAsync
                );

            v.DataContext = vm;
            _targetControl.Content = v;

            try
            {
                await vm!.StartAsync();
            }
            catch (Exception excp)
            {
                _targetControl.Content = excp.Message + Environment.NewLine + excp.StackTrace;
                Logging.LogVS(excp);
            }
        }

        public void SetSteps(
            IChainStep previousStepIfConstructorArgumentExists,
            IChainStep previousStepIfNoConstructorArgumentExists,
            IChainStep nextStep
            )
        {
            if (previousStepIfConstructorArgumentExists is null)
            {
                throw new ArgumentNullException(nameof(previousStepIfConstructorArgumentExists));
            }

            if (previousStepIfNoConstructorArgumentExists is null)
            {
                throw new ArgumentNullException(nameof(previousStepIfNoConstructorArgumentExists));
            }

            if (nextStep is null)
            {
                throw new ArgumentNullException(nameof(nextStep));
            }

            _previousStepIfConstructorArgumentExists = previousStepIfConstructorArgumentExists;
            _previousStepIfNoConstructorArgumentExists = previousStepIfNoConstructorArgumentExists;
            _nextStep = nextStep;
        }

        public Task PreviousAsync()
        {
            if (_choosedParameters.ChoosedConstructor?.Parameters.Length > 0)
            {
                if (_previousStepIfConstructorArgumentExists == null)
                {
                    throw new InvalidOperationException("Previous step is not set");
                }

                return _previousStepIfConstructorArgumentExists.CreateAsync();
            }
            else
            {
                if (_previousStepIfNoConstructorArgumentExists == null)
                {
                    throw new InvalidOperationException("Previous step is not set");
                }

                return _previousStepIfNoConstructorArgumentExists.CreateAsync();
            }

        }

        public Task NextAsync()
        {
            if (_nextStep == null)
            {
                throw new InvalidOperationException("Next step is not set");
            }

            return _nextStep.CreateAsync();
        }

    }
}
