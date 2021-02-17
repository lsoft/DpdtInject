using System;
using System.Diagnostics.CodeAnalysis;
using DpdtInject.Injector.CustomScope;

namespace DpdtInject.Injector.RContext
{
    public abstract class BaseResolutionRequest<TClusterType>
        : IResolutionRequest
    {
        public Type ClusterDeclaredType => typeof(TClusterType);

        public abstract Type RequestedType
        {
            get;
        }

        public bool IsRoot => ParentRequest is null;

        public IResolutionRequest RootRequest
        {
            get
            {
                IResolutionRequest i = this;
                while (!(i.ParentRequest is null))
                {
                    i = i.ParentRequest;
                }

                return i;
            }
        }

        public IResolutionRequest? ParentRequest => ParentTarget?.ParentRequest ?? null;


        public IResolutionTarget? ParentTarget
        {
            get;
        }

        public bool IsGetAllResolution
        {
            get;
        }

        public string? ConstructorArgumentName
        {
            get;
        }

        public CustomScopeObject? ScopeObject
        {
            get;
        }

        protected BaseResolutionRequest(
            bool isGetAllResolution,
            CustomScopeObject? scopeObject
            ) 
        {
            ParentTarget = null;
            IsGetAllResolution = isGetAllResolution;
            ConstructorArgumentName = null;
            ScopeObject = scopeObject;
        }

        protected BaseResolutionRequest(
            IResolutionTarget parentTarget,
            string constructorArgumentName
            )
        {
            ParentTarget = parentTarget;

            IsGetAllResolution = parentTarget.IsGetAllResolution;
            ConstructorArgumentName = constructorArgumentName;
            ScopeObject = parentTarget.ScopeObject;
        }

        public bool TryGetParentRequest(
            [NotNullWhen(true)] out IResolutionRequest? parentRequest
            )
        {
            if (ParentRequest is null)
            {
                parentRequest = null;
                return false;
            }

            parentRequest = ParentRequest;
            return true;
        }

        public bool TryGetParentTarget(
            [NotNullWhen(true)] out IResolutionTarget? parentTarget
            )
        {
            if (ParentTarget is null)
            {
                parentTarget = null;
                return false;
            }

            parentTarget = ParentTarget;
            return true;
        }

    }

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
