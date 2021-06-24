using System;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension.UI.ChainStep
{
    public class MedianeChainStep
    {
        private IChainStep? _previousStep;
        private IChainStep? _nextStep;


        public void SetSteps(
            IChainStep previousStep,
            IChainStep nextStep
            )
        {
            if (previousStep is null)
            {
                throw new ArgumentNullException(nameof(previousStep));
            }

            if (nextStep is null)
            {
                throw new ArgumentNullException(nameof(nextStep));
            }

            _previousStep = previousStep;
            _nextStep = nextStep;
        }


        public Task PreviousAsync()
        {
            if (_previousStep == null)
            {
                throw new InvalidOperationException("Previous step is not set");
            }

            return _previousStep.CreateAsync();
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
