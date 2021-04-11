using System;
using System.Collections.Generic;
using DpdtInject.Injector.Src.CustomScope;
using DpdtInject.Injector.Src.RContext;

namespace DpdtInject.Injector.Src
{
    public interface IResolution
    {
        bool IsRegisteredFrom<T>();

        bool IsRegisteredFrom(System.Type requestedType);

        bool IsRegisteredFromRecursive<T>();



        T Get<T>();

        T Get<T>(CustomScopeObject customScope);



        List<T> GetAll<T>();

        List<T> GetAll<T>(CustomScopeObject customScope);



        object Get(Type requestedType);

        object Get(Type requestedType, CustomScopeObject customScope);



        IEnumerable<object> GetAll(Type requestedType);

        IEnumerable<object> GetAll(Type requestedType, CustomScopeObject customScope);

    }

    public interface IResolution<T>
    {
        T Get(
            IResolutionRequest? resolutionRequest
            );

        List<T> GetAll(IResolutionRequest resolutionRequest);
    }
}
