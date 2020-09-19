using System;

namespace DpdtInject.Injector.Module.RContext
{
    public class ResolutionFrame : IResolutionFrame
    {
        public Type RequestedType
        {
            get;
        }

        public Type TargetType
        {
            get;
        }

        public string? ConstructorArgumentName
        {
            get;
        }

        public ResolutionFrame(
            Type requestedType,
            Type targetType,
            string? constructorArgumentName = null
            )
        {
            if (requestedType is null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            if (targetType is null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            RequestedType = requestedType;
            TargetType = targetType;
            ConstructorArgumentName = constructorArgumentName;
        }
    }
}
