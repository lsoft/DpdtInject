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
using System.Threading;
using DpdtInject.Generator.Producer.Product;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Generator.Producer
{
    internal class ConstantInstanceProducer : IInstanceProducer
    {
        private readonly ITypeInfoProvider _typeInfoProvider;
        private readonly ClusterBindings _clusterBindings;
        private readonly BindingContainerExtender _bindingExtender;

        public ConstantInstanceProducer(
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
            MethodProduct? predicateMethod = null;
            if (_bindingExtender.BindingContainer.IsConditional)
            {
                predicateMethod = new MethodProduct(
                    $"CheckPredicate_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}",
                    _typeInfoProvider.Bool(),
                    (methodName, returnType) => $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}({nameof(IResolutionTarget)} resolutionTarget)
{{
    Func<{nameof(IResolutionTarget)}, bool> predicate = 
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
    {nameof(IResolutionTarget)} resolutionTarget
    )
{{
    return {_bindingExtender.BindingContainer.ConstantSyntax!.GetText()};
}}
");

            var funcMethod = new MethodProduct(
                $"GetInstance_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}{DpdtArgumentWrapperTypeEnum.Func.GetPostfix()}",
                _typeInfoProvider.Func(_bindingExtender.BindingContainer.BindToType),
                (methodName, returnType) => $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(
    {nameof(IResolutionTarget)} resolutionTarget
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
                null
                );

            return product;
        }
    }
}
