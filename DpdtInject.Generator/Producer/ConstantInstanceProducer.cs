using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
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

namespace DpdtInject.Generator.Producer
{
    internal class ConstantInstanceProducer : IInstanceProducer
    {
        private readonly Compilation _compilation;
        private readonly ClusterBindings _clusterBindings;
        private readonly BindingContainerExtender _bindingExtender;

        public ConstantInstanceProducer(
            Compilation compilation,
            ClusterBindings clusterBindings,
            BindingContainerExtender bindingExtender
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (clusterBindings is null)
            {
                throw new ArgumentNullException(nameof(clusterBindings));
            }

            if (bindingExtender is null)
            {
                throw new ArgumentNullException(nameof(bindingExtender));
            }
            _compilation = compilation;
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
            var constructorArgumentProducts = constructorArgumentProducers
                .ConvertAll(p => p.Produce())
                .FindAll(s => !ReferenceEquals(s, ConstructorArgumentProduct.Empty))
                ;
            
            MethodProduct? predicateMethod = null;
            if (_bindingExtender.BindingContainer.IsConditional)
            {
                predicateMethod = new MethodProduct(
                    $"CheckPredicate_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}",
                    _compilation.Bool(),
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

            var constantInstanceName = $"_constantInstance_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}";

            var retrieveObjectMethod = new MethodProduct(
                $"GetInstance_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}",
                _bindingExtender.BindingContainer.BindToType,
                (methodName, returnType) => $@"
private readonly {_bindingExtender.BindingContainer.BindToType.ToDisplayString()} {constantInstanceName} = {_bindingExtender.BindingContainer.ConstantSyntax!.GetText()};

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(
    IResolutionTarget resolutionTarget
    )
{{
    return {constantInstanceName};
}}
");

            var funcMethod = new MethodProduct(
                $"GetInstance_{_bindingExtender.BindingContainer.GetStableSuffix()}_{ _bindingExtender.BindingContainer.BindToType.ToDisplayString().EscapeSpecialTypeSymbols()}{DpdtArgumentWrapperTypeEnum.Func.GetPostfix()}",
                _compilation.Func(_bindingExtender.BindingContainer.BindToType),
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
                null
                );

            return product;
        }
    }
}
