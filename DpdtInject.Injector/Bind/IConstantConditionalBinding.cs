using System;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Injector.Bind
{
    public interface IConstantConditionalBinding
    {
        void When(
            Func<IResolutionTarget, bool> predicate
            );
    }
}