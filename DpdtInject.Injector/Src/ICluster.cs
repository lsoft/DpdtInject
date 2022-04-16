using System;
using DpdtInject.Injector.Src.CustomScope;
using DpdtInject.Injector.Src.RContext;

namespace DpdtInject.Injector.Src
{
    /// <summary>
    /// Dpdt cluster, a resolution source. Can be hierarchied.
    /// </summary>
    public interface ICluster
        :
            IDisposable,
#if !DPDT_INTERNAL_SUPPRESS_ASYNC_DISPOSABLE
            IAsyncDisposable,
#endif
            IResolution
    {
        /// <summary>
        /// A method for internal use only. Please DO NOT use it.
        /// </summary>
        T GetToChild<T>(IResolutionRequest resolutionRequest);

        /// <summary>
        /// Create a custom scope object.
        /// </summary>
        CustomScopeObject CreateCustomScope();
    }
}
