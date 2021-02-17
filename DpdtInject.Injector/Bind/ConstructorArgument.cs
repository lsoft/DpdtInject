using System;

namespace DpdtInject.Injector.Bind
{
    public class ConstructorArgument
    {
        public string ArgumentName
        {
            get;
        }

        public object ArgumentValue
        {
            get;
        }

        public ConstructorArgument(
            string argumentName,
            object argumentValue
            )
        {
            if (argumentName is null)
            {
                throw new ArgumentNullException(nameof(argumentName));
            }

            if (argumentValue is null)
            {
                throw new ArgumentNullException(nameof(argumentValue));
            }

            ArgumentName = argumentName;
            ArgumentValue = argumentValue;
        }
    }

}
