using System;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Injector.Bind
{
    public interface IConditionalBinding
    {
        IConfigureBinding When(
            Func<IResolutionTarget, bool> predicate
            );
    }
}