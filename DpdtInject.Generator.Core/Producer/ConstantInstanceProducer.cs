using DpdtInject.Generator.Core.ArgumentWrapper;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.TypeInfo;
using System;
using DpdtInject.Generator.Core.Producer.Product;
using DpdtInject.Injector.Src;

namespace DpdtInject.Generator.Core.Producer
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
            IMethodProduct? predicateMethod = null;
            if (_bindingExtender.BindingContainer.IsConditional)
            {
                predicateMethod = MethodProductFactory.Create(
                    $"CheckPredicate_{_bindingExtender.BindingContainer.BindToType.Name}_{_bindingExtender.BindingContainer.GetStableSuffix()}",
                    new TypeMethodResult(_typeInfoProvider.Bool()),
                    (methodName, returnType) => $@"
{GN.AggressiveInlining}
private {returnType} {methodName}({GN.IResolutionTarget} resolutionTarget)
{{
    global::System.Func<{GN.IResolutionTarget}, bool> predicate = 
        {_bindingExtender.BindingContainer.WhenArgumentClause}
        ;

    var result = predicate(resolutionTarget);
    return result;
}}
");
            }

            var retrieveObjectMethod = MethodProductFactory.Create(
                $"GetInstance_{_bindingExtender.BindingContainer.BindToType.Name}_{_bindingExtender.BindingContainer.GetStableSuffix()}",
                new TypeMethodResult(_bindingExtender.BindingContainer.BindToType),
                (methodName, returnType) => $@"
{GN.AggressiveInlining}
private {returnType} {methodName}(
    {GN.IResolutionTarget} resolutionTarget
    )
{{
    return {_bindingExtender.BindingContainer.ConstantSyntax!.GetText()};
}}
");

            var funcMethod = MethodProductFactory.Create(
                $"GetInstance_{_bindingExtender.BindingContainer.BindToType.Name}_{_bindingExtender.BindingContainer.GetStableSuffix()}{DpdtArgumentWrapperTypeEnum.Func.GetPostfix()}",
                new TypeMethodResult(_typeInfoProvider.Func(_bindingExtender.BindingContainer.BindToType)),
                (methodName, returnType) => $@"
{GN.AggressiveInlining}
private {returnType} {methodName}(
    {GN.IResolutionTarget} resolutionTarget
    )
{{
    return () => {retrieveObjectMethod.GetWrappedMethodName(DpdtArgumentWrapperTypeEnum.None)}(resolutionTarget);
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
