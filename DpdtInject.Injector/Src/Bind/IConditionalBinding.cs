using System;
using DpdtInject.Injector.Src.RContext;

namespace DpdtInject.Injector.Src.Bind
{
    public interface IConditionalBinding
    {
        IConfigureBinding When(
            Func<IResolutionTarget, bool> predicate
            );
    }
}
