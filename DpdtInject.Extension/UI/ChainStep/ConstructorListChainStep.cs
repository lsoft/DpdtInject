using DpdtInject.Extension.Shared;
using DpdtInject.Extension.UI.Control;
using DpdtInject.Extension.UI.ViewModel.Add;
using System;
using System.Windows.Controls;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension.UI.ChainStep
{
    public class ConstructorListChainStep : IChainStep
    {
        private readonly ContentControl _targetControl;
        private readonly ChoosedParameters _choosedParameters;
        
        private IChainStep? _nextStepIfConstructorArgumentsExists;
        private IChainStep? _nextStepIfNoConstructorArgumentsExists;

        public ConstructorListChainStep(
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

        public void SetSteps(
            IChainStep nextStepIfConstructorArgumentsExists,
            IChainStep nextStepIfNoConstructorArgumentsExists
            )
        {
            if (nextStepIfConstructorArgumentsExists is null)
            {
                throw new ArgumentNullException(nameof(nextStepIfConstructorArgumentsExists));
            }

            if (nextStepIfNoConstructorArgumentsExists is null)
            {
                throw new ArgumentNullException(nameof(nextStepIfNoConstructorArgumentsExists));
            }

            _nextStepIfConstructorArgumentsExists = nextStepIfConstructorArgumentsExists;
            _nextStepIfNoConstructorArgumentsExists = nextStepIfNoConstructorArgumentsExists;
        }

        public async Task CreateAsync()
        {
            var v = new ConstructorListControl();

            var vm = new ConstructorListViewModel(
                _choosedParameters,
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

        public Task NextAsync()
        {
            if (_choosedParameters.ChoosedConstructor?.Parameters.Length > 0)
            {
                if (_nextStepIfConstructorArgumentsExists == null)
                {
                    throw new InvalidOperationException("Next step is not set");
                }

                return _nextStepIfConstructorArgumentsExists.CreateAsync();
            }
            else
            {
                if (_nextStepIfNoConstructorArgumentsExists == null)
                {
                    throw new InvalidOperationException("Next step is not set");
                }

                return _nextStepIfNoConstructorArgumentsExists.CreateAsync();
            }
        }
    }
}
