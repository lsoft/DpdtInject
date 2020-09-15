using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Injector.Module.Bind
{
    //public interface IBindConfigurationProvider
    //{
    //    BindConfiguration CreateConfiguration();
    //}

    public interface IToBinding
    {
        IScopeBinding To<T>();
    }

    public interface IScopeBinding //: IBindConfigurationProvider
    {
        //IConfigureAndConditionalBinding WithTransientScope();

        IConfigureAndConditionalBinding WithSingletonScope();

        //IConfigureAndConditionalBinding WithConstScope(object? constant);
    }

    public interface IConfigureAndConditionalBinding :
        IConditionalBinding, IConfigureBinding//, IBindConfigurationProvider
    {

    }

    public interface IConditionalBinding //: IBindConfigurationProvider
    {
        IConfigureBinding When(
            Func<ResolutionContext, bool> predicate
            );
    }

    public interface IConfigureBinding //: IBindConfigurationProvider
    {
        IConfigureBinding Configure(
            ConstructorArgument argument
            );
    }


    public class DefineBindingNode
        : IToBinding, IScopeBinding, IConfigureAndConditionalBinding, IConfigureBinding//, IBindConfigurationProvider
    {
        private readonly Dictionary<string, ConstructorArgument> _constructorArguments = new Dictionary<string, ConstructorArgument>();
        
        private bool _conditionalBinding = false;

        //private Func<IEmptyContext, bool>? _predicate;
        //private object? _constant;

        //public string Name
        //{
        //    get;
        //}

        public IReadOnlyList<Type> BindsFrom
        {
            get;
        }

        public string BindsFromFullName => string.Join(",", BindsFrom.Select(b => b.FullName));

        public Type BindTo
        {
            get;
            private set;
        }

        public BindScopeEnum BindScope
        {
            get;
            private set;
        }

        //public DpdtIdempotentStatusEnum IdempotentStatus
        //{
        //    get;
        //    private set;
        //} = DpdtIdempotentStatusEnum.Idempotent;


        public bool IsCompleted => BindTo != null;

        public DefineBindingNode(
            //string name,
            IReadOnlyList<Type> bindFrom
            )
        {
            //if (name is null)
            //{
            //    throw new ArgumentNullException(nameof(name));
            //}

            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            //Name = name;
            BindsFrom = bindFrom;
            BindTo = null!;
            BindScope = BindScopeEnum.Transient;
        }

        public IScopeBinding To<T>()
        {
            BindTo = typeof(T);

            return this;
        }

        //public IConfigureAndConditionalBinding WithTransientScope()
        //{
        //    if (BindTo == null)
        //    {
        //        throw new DpdtException(
        //            DpdpExceptionTypeEnum.IncorrectBinding,
        //            $"Incorrect binding found {BindsFromFullName}"
        //            );
        //    }

        //    BindScope = BindScopeEnum.Transient;

        //    return this;
        //}

        public IConfigureAndConditionalBinding WithSingletonScope()
        {
            if (BindTo == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding,
                    $"Incorrect binding found {BindsFromFullName}"
                    );
            }

            BindScope = BindScopeEnum.Singleton;

            return this;
        }

        //public IConfigureAndConditionalBinding WithConstScope(object? constant)
        //{
        //    if (BindTo == null)
        //    {
        //        throw new DpdtException(
        //            DpdpExceptionTypeEnum.IncorrectBinding,
        //            $"Incorrect binding found {BindsFromFullName}"
        //            );
        //    }

        //    if(constant is not null)
        //    {
        //        var constantType = constant.GetType();
        //        if (constantType != BindTo && !constantType.IsSubclassOf(BindTo))
        //        {
        //            throw new DpdtException(
        //                DpdpExceptionTypeEnum.IncorrectBinding,
        //                $"Incorrect binding found {BindsFromFullName} to {BindTo.FullName} with constant of type {constant.GetType().FullName}"
        //                );
        //        }
        //    }

        //    BindScope = BindScopeEnum.Constant;

        //    _constant = constant;

        //    return this;
        //}

        public IConfigureBinding When(
            Func<ResolutionContext, bool> predicate
            )
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            //    IdempotentStatus = idempotentStatus;

            //    _predicate = predicate;

            _conditionalBinding = true;

            return this;
        }

        public IConfigureBinding Configure(
            ConstructorArgument argument
            )
        {
            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }
            //if (_constant != null)
            //{
            //    throw new DpdtException(
            //        DpdpExceptionTypeEnum.ConstantCantHaveConstructorArguments,
            //        $"Constructor arguments for constant scope are pointless"
            //        );
            //}

            _constructorArguments[argument.ArgumentName] = argument;

            return this;
        }

        //public BindConfiguration CreateConfiguration()
        //{
        //    var bindNode = new BindNode(
        //        BindsFrom,
        //        BindTo,
        //        BindScope
        //        //Name
        //        );

        //    switch (BindScope)
        //    {
        //        case BindScopeEnum.Transient:
        //            throw new DpdtException(DpdtExceptionTypeEnum.UnknownScope, $"Unknown scope {BindScope}");
        //            //return new BindConfiguration(bindNode, _predicate, _constructorArguments, IdempotentStatus);
        //        case BindScopeEnum.Singleton:
        //            return new BindConfiguration(bindNode, _conditionalBinding, /*_predicate,*/ _constructorArguments/*, IdempotentStatus*/);
        //        case BindScopeEnum.Constant:
        //            throw new DpdtException(DpdtExceptionTypeEnum.UnknownScope, $"Unknown scope {BindScope}");
        //            //return new BindConfiguration(bindNode, _predicate, _constant, IdempotentStatus);
        //        default:
        //            throw new DpdtException(DpdtExceptionTypeEnum.UnknownScope, $"Unknown scope {BindScope}");
        //    }

        //}
    }
}
