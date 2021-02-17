using DpdtInject.Injector.CustomScope;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Injector
{
    public interface ICluster
        : IDisposable, IResolution
    {
        T GetToChild<T>(IResolutionRequest resolutionRequest);

        List<Exception> TrySafelyDispose(
            );

        CustomScopeObject CreateCustomScope();
    }

    public interface IResolution
    {
        bool IsRegisteredFrom<T>();

        bool IsRegisteredFrom(System.Type requestedType);



        T Get<T>();

        T Get<T>(CustomScopeObject customScope);



        List<T> GetAll<T>();

        List<T> GetAll<T>(CustomScopeObject customScope);



        object Get(Type requestedType);

        object Get(Type requestedType, CustomScopeObject customScope);



        IEnumerable<object> GetAll(Type requestedType);

        IEnumerable<object> GetAll(Type requestedType, CustomScopeObject customScope);

    }

    public interface IResolutionFast<TR>
    {
        TR GetFast(TR unused);
    }

    public interface IResolution<T>
    {
        T Get(
            IResolutionRequest? resolutionRequest
            );

        List<T> GetAll(IResolutionRequest resolutionRequest);
    }
}
