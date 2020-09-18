using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Injector.Module.Bind
{
    public interface IToBinding
    {
        IScopeBinding To<T>();
    }

    public interface IScopeBinding 
    {
        IConfigureAndConditionalBinding WithTransientScope();

        IConfigureAndConditionalBinding WithSingletonScope();

        //IConfigureAndConditionalBinding WithConstScope(object? constant);
    }

    public interface IConfigureAndConditionalBinding :
        IConditionalBinding, IConfigureBinding
    {

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
