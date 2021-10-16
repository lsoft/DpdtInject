using System;

namespace DpdtInject.Generator.Core.Binding
{
    public class ExtenderAndTypePair
    {
        public BindingExtender BindingExtender
        {
            get;
        }
        
        public DetectedMethodArgument ConstructorArgument
        {
            get;
        }

        public ExtenderAndTypePair(
            BindingExtender bindingExtender,
            DetectedMethodArgument constructorArgument
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