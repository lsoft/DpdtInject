using System;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Injector.Bind
{
    public interface IToOrConstantBinding
    {
        IScopeBinding To<T>();

        IToFactoryBinding ToIsolatedFactory<TFactory>();

        IConstantConditionalBinding WithConstScope<T>(T? constant);
    }

    public interface IToFactoryBinding
    {
        IScopeBinding WithPayload<TPayload>();
    }

    public interface IScopeBinding
    {
        IConfigureAndConditionalBinding WithTransientScope();

        IConfigureAndConditionalBinding WithSingletonScope();

        IConfigureAndConditionalBinding WithCustomScope();
    }


    public interface IConfigureAndConditionalBinding :
        IConditionalBinding, IConfigureBinding
    {

    }
    public interface IConstantConditionalBinding
    {
        void When(
            Func<IResolutionTarget, bool> predicate
            );
    }

    public interface IConditionalBinding
    {
        IConfigureBinding When(
            Func<IResolutionTarget, bool> predicate
            );
    }

    public interface IConfigureBinding
    {
        IConfigureBinding Configure(
            ConstructorArgument argument
            );
    }
}
