using System;
using DpdtInject.Injector.Src.CustomScope;
using DpdtInject.Injector.Src.RContext;

namespace DpdtInject.Injector.Src
{
    public interface ICluster
        : IDisposable, IResolution
    {
        T GetToChild<T>(IResolutionRequest resolutionRequest);

        CustomScopeObject CreateCustomScope();
    }
}
