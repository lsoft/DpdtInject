using System;
using DpdtInject.Injector.Bind.Settings;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Injector.Bind
{
    public interface IConstantConditionalBinding
    {
        IConstantConditionalBinding Setup<T>()
            where T : ISetting;

        void When(
            Func<IResolutionTarget, bool> predicate
            );
    }
}