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

    internal class TransientInstanceProducer : IInstanceProducer
    {
        private readonly ITypeInfoProvider _typeInfoProvider;
        private readonly ClusterBindings _clusterBindings;
        private readonly BindingContainerExtender _bindingExtender;

        public TransientInstanceProducer(
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

            var retrieveObjectMethod = new MethodProduct(
                $"GetInstance_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}",
                _bindingExtender.BindingContainer.BindToType,
                (methodName, returnType) => $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(
    IResolutionTarget resolutionTarget
    )
{{
    return new {_bindingExtender.BindingContainer.BindToType.ToDisplayString()}(
        {caps.Join(p => p.ResolveConstructorArgumentClause, ",")}
        );
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

            var product = new InstanceProduct(
                _bindingExtender,
                predicateMethod,
                retrieveObjectMethod,
                funcMethod,
                null,
                utps
                );

            return product;
        }
    }
}
