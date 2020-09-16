using System;

namespace DpdtInject.Injector.Module.RContext
{
    public class ResolutionFrame : IResolutionFrame
    {
        public Type RequestedType
        {
            get;
        }

        public string? ConstructorArgumentName
        {
            get;
        }

        public ResolutionFrame(
            Type requestedType,
            string? constructorArgumentName = null
            )
        {
            if (requestedType is null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            RequestedType = requestedType;
            ConstructorArgumentName = constructorArgumentName;
        }
    }
}
