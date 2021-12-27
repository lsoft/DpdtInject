using System;
using DpdtInject.Injector.Src.CustomScope;
using DpdtInject.Injector.Src.RContext;

namespace DpdtInject.Injector.Src
{
    /// <summary>
    /// Dpdt cluster. Can be hierarchied and be a resolution source.
    /// </summary>
    public interface ICluster
        : IDisposable, IResolution
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
