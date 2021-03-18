using System;
using System.Collections.Generic;
using DpdtInject.Injector.CustomScope;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Injector
{
    public interface ICluster
        : IDisposable, IResolution
    {
        T GetToChild<T>(IResolutionRequest resolutionRequest);

        CustomScopeObject CreateCustomScope();
    }
}