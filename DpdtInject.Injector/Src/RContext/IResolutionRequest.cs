using System;
using System.Diagnostics.CodeAnalysis;
using DpdtInject.Injector.CustomScope;

namespace DpdtInject.Injector.RContext
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
