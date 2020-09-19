using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Injector.Module.Bind
{
    public interface IToOrContantBinding
    {
        IScopeBinding To<T>();

        IConstantConditionalBinding WithConstScope<T>(T? constant);
    }

    public interface IScopeBinding 
    {
        IConfigureAndConditionalBinding WithTransientScope();

        IConfigureAndConditionalBinding WithSingletonScope();
    }

    public interface IConfigureAndConditionalBinding :
        IConditionalBinding, IConfigureBinding
    {

    }

    public interface IConstantConditionalBinding
    {
        void When(
            Func<IResolutionContext, bool> predicate
            );
    }

    public interface IConditionalBinding
    {
        IConfigureBinding When(
            Func<IResolutionContext, bool> predicate
            );
    }

    public interface IConfigureBinding
    {
        IConfigureBinding Configure(
            ConstructorArgument argument
            );
    }
}
