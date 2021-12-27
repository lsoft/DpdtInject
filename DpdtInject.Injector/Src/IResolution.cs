using System;
using System.Collections.Generic;
using DpdtInject.Injector.Src.CustomScope;
using DpdtInject.Injector.Src.RContext;

namespace DpdtInject.Injector.Src
{
    public interface IResolution
    {
        /// <summary>
        /// Check if a Dpdt cluster has an appropriate registered service.
        /// </summary>
        bool IsRegisteredFrom<T>();

        /// <summary>
        /// Check if a Dpdt cluster has an appropriate registered service.
        /// </summary>
        bool IsRegisteredFrom(System.Type requestedType);

        /// <summary>
        /// Internal method. Please do not use it.
        /// </summary>
        bool IsRegisteredFromRecursive<T>();


        /// <summary>
        /// Get a registered service.
        /// <see cref="DpdtInject.Injector.Src.Excp.DpdtException"/> can be raised if any errors occurs.
        /// </summary>
        T Get<T>();

        /// <summary>
        /// Get a registered service in a custom scope.
        /// <see cref="DpdtInject.Injector.Src.Excp.DpdtException"/> can be raised if any errors occurs.
        /// </summary>
        T Get<T>(CustomScopeObject customScope);



        /// <summary>
        /// Get a list of registered services.
        /// <see cref="DpdtInject.Injector.Src.Excp.DpdtException"/> can be raised if any errors occurs.
        /// </summary>
        List<T> GetAll<T>();

        /// <summary>
        /// Get a list of registered services in a custom scope.
        /// <see cref="DpdtInject.Injector.Src.Excp.DpdtException"/> can be raised if any errors occurs.
        /// </summary>
        List<T> GetAll<T>(CustomScopeObject customScope);



        /// <summary>
        /// Get a registered service.
        /// <see cref="DpdtInject.Injector.Src.Excp.DpdtException"/> can be raised if any errors occurs.
        /// </summary>
        object Get(Type requestedType);

        /// <summary>
        /// Get a registered service in a custom scope.
        /// <see cref="DpdtInject.Injector.Src.Excp.DpdtException"/> can be raised if any errors occurs.
        /// </summary>
        object Get(Type requestedType, CustomScopeObject customScope);



        /// <summary>
        /// Get a list of registered services.
        /// <see cref="DpdtInject.Injector.Src.Excp.DpdtException"/> can be raised if any errors occurs.
        /// </summary>
        IEnumerable<object> GetAll(Type requestedType);

        /// <summary>
        /// Get a list of registered services in a custom scope.
        /// <see cref="DpdtInject.Injector.Src.Excp.DpdtException"/> can be raised if any errors occurs.
        /// </summary>
        IEnumerable<object> GetAll(Type requestedType, CustomScopeObject customScope);

    }

    /// <summary>
    /// Internal interface. Please do not use it.
    /// </summary>
    public interface IResolution<T>
    {
        T Get(
            IResolutionRequest? resolutionRequest
            );

        List<T> GetAll(IResolutionRequest resolutionRequest);
    }
}
