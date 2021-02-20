using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DpdtInject.Generator.Producer.Product;

namespace DpdtInject.Generator.Producer
{
    internal class SingletonInstanceProducer : IInstanceProducer

    {
        private readonly ITypeInfoProvider _typeInfoProvider;
        private readonly ClusterBindings _clusterBindings;
        private readonly BindingContainerExtender _bindingExtender;

        public SingletonInstanceProducer(
            ITypeInfoProvider typeInfoProvider,
            ClusterBindings clusterBindings,
            BindingContainerExtender bindingExtender
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            if (clusterBindings is null)
            {
                throw new ArgumentNullException(nameof(clusterBindings));
            }

            if (bindingExtender is null)
            {
                throw new ArgumentNullException(nameof(bindingExtender));
            }
            _typeInfoProvider = typeInfoProvider;
            _clusterBindings = clusterBindings;
            _bindingExtender = bindingExtender;
        }

        public InstanceProduct Produce()
        {
            var constructorArgumentProducers =
                _bindingExtender.BindingContainer.ConstructorArguments.ConvertAll(
                    bft => new ConstructorArgumentProducer(
                        _clusterBindings,
                        _bindingExtender,
                        bft
                        )
                    );

            var (caps, utps) = constructorArgumentProducers.Produce();

            MethodProduct? predicateMethod = null;
            if (_bindingExtender.BindingContainer.IsConditional)
            {
                predicateMethod = new MethodProduct(
                    $"CheckPredicate_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}",
                    _typeInfoProvider.Bool(),
                    (methodName, returnType) => $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(IResolutionTarget resolutionTarget)
{{
    Func<IResolutionTarget, bool> predicate = 
        {_bindingExtender.BindingContainer.WhenArgumentClause}
        ;

    var result = predicate(resolutionTarget);
    return result;
}}
");
            }

            var singletonInstanceName = $"_singletonInstance_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}";
            var lockerName = $"_locker_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}";

            var retrieveObjectMethod = new MethodProduct(
                $"GetInstance_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}",
                _bindingExtender.BindingContainer.BindToType,
                (methodName, returnType) => $@"
private volatile {_bindingExtender.BindingContainer.BindToType.ToDisplayString()} {singletonInstanceName} = null;
private readonly System.Object {lockerName} = new System.Object();

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(
    IResolutionTarget resolutionTarget
    )
{{
    if({singletonInstanceName} is null)
    {{
        lock({lockerName})
        {{
            if({singletonInstanceName} is null)
            {{
                {singletonInstanceName} = new {_bindingExtender.BindingContainer.BindToType.ToDisplayString()}(
                    {caps.Join(p => p.ResolveConstructorArgumentClause, ",")}
                    );
            }}
        }}
    }}

    return {singletonInstanceName};
}}
");

            var funcMethod = new MethodProduct(
                $"GetInstance_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}{DpdtArgumentWrapperTypeEnum.Func.GetPostfix()}",
                _typeInfoProvider.Func(_bindingExtender.BindingContainer.BindToType),
                (methodName, returnType) => $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(
    IResolutionTarget resolutionTarget
    )
{{
    return () => {retrieveObjectMethod.GetMethodName(DpdtArgumentWrapperTypeEnum.None)}(resolutionTarget);
}}
");

            var disposeMethodInvoke = new MethodProduct(
                $"DisposeInstance_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}",
                _typeInfoProvider.Void(),
                (methodName, returnType) => $@"
private {returnType.ToDisplayString()} {methodName}(
    )
{{
    if({singletonInstanceName} is {nameof(IDisposable)} d)
    {{
        d.{nameof(IDisposable.Dispose)}();
    }}
}}
");

            var product = new InstanceProduct(
                _bindingExtender,
                predicateMethod,
                retrieveObjectMethod,
                funcMethod,
                disposeMethodInvoke,
                utps
                );

            return product;
        }
    }
}
