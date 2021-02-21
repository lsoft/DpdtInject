using System;
using DpdtInject.Injector.CustomScope;

namespace DpdtInject.Injector.RContext
{
    public class ResolutionRequest<TClusterType> : BaseResolutionRequest<TClusterType>
    {
        public override Type RequestedType
        {
            get;
        }

        public ResolutionRequest(
            bool isGetAllResolution,
            Type requestedType,
            CustomScopeObject? scopeObject = null
            ) : base(isGetAllResolution, scopeObject)
        {
            RequestedType = requestedType;
        }

        public ResolutionRequest(
            IResolutionTarget parentTarget,
            string constructorArgumentName
            ) : base(parentTarget, constructorArgumentName)
        {
            RequestedType = parentTarget.ParentRequest.RequestedType;
        }

    }

    public class ResolutionRequest<TClusterType, TRequestedType> : BaseResolutionRequest<TClusterType>
    {
        public override Type RequestedType => typeof(TRequestedType);

        public ResolutionRequest(
            bool isGetAllResolution,
            CustomScopeObject? scopeObject = null
            ) : base(isGetAllResolution, scopeObject)
        {
        }

        public ResolutionRequest(
            IResolutionTarget parentTarget,
            string constructorArgumentName
            ) : base(parentTarget, constructorArgumentName)
        {
        }
    }
}
