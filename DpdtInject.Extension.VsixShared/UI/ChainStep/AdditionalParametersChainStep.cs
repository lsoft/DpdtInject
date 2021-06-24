using DpdtInject.Extension.Shared;
using DpdtInject.Extension.UI.Control;
using DpdtInject.Extension.UI.ViewModel.Add;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows.Controls;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension.UI.ChainStep
{

    public class AdditionalParametersChainStep : IChainStep
    {
        private readonly DialogWindow _window;
        private readonly ContentControl _targetControl;
        private readonly ChoosedParameters _choosedParameters;
        
        private IChainStep? _previousStep;


        public AdditionalParametersChainStep(
            DialogWindow window,
            ContentControl targetControl,
            ChoosedParameters choosedParameters
            )
        {
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (targetControl is null)
            {
                throw new ArgumentNullException(nameof(targetControl));
            }

            if (choosedParameters is null)
            {
                throw new ArgumentNullException(nameof(choosedParameters));
            }
            _window = window;
            _targetControl = targetControl;
            _choosedParameters = choosedParameters;
        }

        public void SetSteps(
            IChainStep previousStep
            //IChainStep nextStep
            )
        {
            if (previousStep is null)
            {
                throw new ArgumentNullException(nameof(previousStep));
            }

            //if (nextStep is null)
            //{
            //    throw new ArgumentNullException(nameof(nextStep));
            //}

            _previousStep = previousStep;
            //_nextStep = nextStep;
        }



        public async Task CreateAsync()
        {
            var v = new AdditionalParametersControl();

            var vm = new AdditionalParametersViewModel(
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

        public Task PreviousAsync()
        {
            if (_previousStep == null)
            {
                throw new InvalidOperationException("Previous step is not set");
            }

            return _previousStep.CreateAsync();
        }

        public async Task NextAsync()
        {
            await _choosedParameters.InsertBindingAsync();

            _window.Close();
        }

    }
}
