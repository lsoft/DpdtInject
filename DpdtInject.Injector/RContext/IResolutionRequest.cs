using DpdtInject.Injector.CustomScope;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Injector.Module.RContext
{
    public interface IResolutionRequest
    {
        Type ClusterDeclaredType 
        { 
            get; 
        }

        Type RequestedType
        {
            get;
        }


        bool IsRoot
        {
            get;
        }


        IResolutionRequest? ParentRequest
        {
            get;
        }

        IResolutionTarget? ParentTarget
        {
            get;
        }

        bool IsGetAllResolution
        {
            get;
        }

        string? ConstructorArgumentName
        {
            get;
        }

        CustomScopeObject? ScopeObject
        {
            get;
        }

        bool TryGetParentRequest(
            [NotNullWhen(true)] out IResolutionRequest? parentRequest
            );

        bool TryGetParentTarget(
            [NotNullWhen(true)] out IResolutionTarget? parentTarget
            );
    }
}
