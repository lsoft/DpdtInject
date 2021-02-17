﻿using System;
using System.Diagnostics.CodeAnalysis;
using DpdtInject.Injector.CustomScope;

namespace DpdtInject.Injector.RContext
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
