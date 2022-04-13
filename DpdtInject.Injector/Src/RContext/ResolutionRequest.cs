using System;
using DpdtInject.Injector.Src.CustomScope;

namespace DpdtInject.Injector.Src.RContext
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
            CustomScopeObject scopeObject
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
            CustomScopeObject scopeObject
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
