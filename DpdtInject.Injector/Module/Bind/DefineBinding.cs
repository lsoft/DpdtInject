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

        IConstantClusterBinding WithConstScope<T>(T? constant);
    }

    public interface IScopeBinding 
    {
        IClusterBinding WithTransientScope();

        IClusterBinding WithSingletonScope();
    }

    public interface IClusterBinding
    {
        IConfigureAndConditionalBinding InCluster<T>();
    }


    public interface IConfigureAndConditionalBinding :
        IConditionalBinding, IConfigureBinding
    {

    }

    public interface IConstantClusterBinding
    {
        IConstantConditionalBinding InCluster<T>();
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
