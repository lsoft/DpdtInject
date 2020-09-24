using System;

namespace DpdtInject.Injector.Module.RContext
{
    public class ResolutionFrame : IResolutionFrame
    {
        public Type ClusterDeclaredType
        {
            get;
        }

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
            Type clusterDeclaredType,
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
            ClusterDeclaredType = clusterDeclaredType;
            RequestedType = requestedType;
            TargetType = targetType;
            ConstructorArgumentName = constructorArgumentName;
        }
    }
}
