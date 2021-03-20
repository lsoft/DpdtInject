using DpdtInject.Extension.Shared;
using DpdtInject.Extension.Shared.Dto;
using DpdtInject.Extension.UI.ChainStep;
using DpdtInject.Extension.UI.Control;
using DpdtInject.Extension.UI.ViewModel.Add;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension.UI.ChainStep
{
    public class ConstructorListChainStep : IChainStep
    {
        private readonly ContentControl _targetControl;
        private readonly ChoosedParameters _choosedParameters;
        
        private IChainStep? _nextStep;

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
            IChainStep nextStep
            )
        {
            if (nextStep is null)
            {
                throw new ArgumentNullException(nameof(nextStep));
            }

            _nextStep = nextStep;
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
            if (_nextStep == null)
            {
                throw new InvalidOperationException("Next step is not set");
            }

            return _nextStep.CreateAsync();
        }
    }
}
