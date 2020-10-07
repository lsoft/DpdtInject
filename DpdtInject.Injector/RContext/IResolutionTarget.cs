using DpdtInject.Injector.CustomScope;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Injector.Module.RContext
{
    public interface IResolutionTarget
    {
        bool IsRoot
        {
            get;
        }

        public Type ClusterDeclaredType
        {
            get;
        }

        public Type TargetType
        {
            get;
        }

        bool IsGetAllResolution
        {
            get;
        }

        CustomScopeObject? ScopeObject
        {
            get;
        }

        IResolutionTarget? ParentTarget
        {
            get;
        }

        IResolutionRequest ParentRequest
        {
            get;
        }

        bool TryGetParentTarget(
            [NotNullWhen(true)] out IResolutionTarget? parentTarget
            );
    }
}
