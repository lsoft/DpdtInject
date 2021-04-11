using System;
using System.Diagnostics.CodeAnalysis;
using DpdtInject.Injector.Src.CustomScope;

namespace DpdtInject.Injector.Src.RContext
{
    public class ResolutionTarget<TClusterType, TTargetType>
        : IResolutionTarget
    {
        public bool IsRoot => ParentTarget is null;

        public Type ClusterDeclaredType => typeof(TClusterType);

        public Type TargetType => typeof(TTargetType);

        public bool IsGetAllResolution => ParentRequest.IsGetAllResolution;

        public CustomScopeObject? ScopeObject => ParentRequest.ScopeObject;


        public IResolutionRequest RootRequest
        {
            get
            {
                IResolutionRequest i = this.ParentRequest;
                while (!(i.ParentRequest is null))
                {
                    i = i.ParentRequest;
                }

                return i;
            }
        }

        public IResolutionTarget RootTarget
        {
            get
            {
                IResolutionTarget frame = this;
                while (!(frame.ParentTarget is null))
                {
                    frame = frame.ParentTarget;
                }

                return frame;
            }
        }

        public IResolutionTarget? ParentTarget => ParentRequest.ParentTarget ?? null;

        public IResolutionRequest ParentRequest
        {
            get;
        }


        public ResolutionTarget(
            IResolutionRequest parentRequest
            )
        {
            if (parentRequest is null)
            {
                throw new ArgumentNullException(nameof(parentRequest));
            }

            ParentRequest = parentRequest;
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
}
