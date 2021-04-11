using System;
using DpdtInject.Injector.Src.Bind.Settings;
using DpdtInject.Injector.Src.RContext;

namespace DpdtInject.Injector.Src.Bind
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
