using System;

namespace DpdtInject.Generator.Binding
{
    public class ExtenderAndTypePair
    {
        public BindingContainerExtender BindingExtender
        {
            get;
        }
        
        public DetectedConstructorArgument ConstructorArgument
        {
            get;
        }

        public ExtenderAndTypePair(
            BindingContainerExtender bindingExtender,
            DetectedConstructorArgument constructorArgument
            )
        {
            if (bindingExtender is null)
            {
                throw new ArgumentNullException(nameof(bindingExtender));
            }

            if (constructorArgument is null)
            {
                throw new ArgumentNullException(nameof(constructorArgument));
            }

            BindingExtender = bindingExtender;
            ConstructorArgument = constructorArgument;
        }

    }
}