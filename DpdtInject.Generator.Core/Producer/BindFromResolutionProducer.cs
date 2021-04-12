using DpdtInject.Generator.Core.ArgumentWrapper;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer.Product;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Core.Producer.Product.Tuple;
using DpdtInject.Injector.Src.Bind.Settings.Wrapper;
using DpdtInject.Injector.Src.RContext;
using DpdtInject.Injector.Src;
using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Generator.Core.Producer
{
    internal class BindFromResolutionProducer
    {
        private readonly ITypeInfoProvider _typeInfoProvider;

        public ClusterBindings ClusterBindings 
        { 
            get; 
        }
        public ITypeSymbol BindFrom
        {
            get;
        }
        public IReadOnlyList<InstanceProduct> InstanceProducts
        { 
            get; 
        }

        public BindFromResolutionProducer(
            ITypeInfoProvider typeInfoProvider,
            ClusterBindings clusterBindings,
            ITypeSymbol bindFrom,
            IReadOnlyList<InstanceProduct> instanceProducts
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

            if (bindFrom is null)
            {
                throw new System.ArgumentNullException(nameof(bindFrom));
            }

            if (instanceProducts is null)
            {
                throw new ArgumentNullException(nameof(instanceProducts));
            }
            _typeInfoProvider = typeInfoProvider;
            ClusterBindings = clusterBindings;
            BindFrom = bindFrom;
            InstanceProducts = instanceProducts;
        }


        public InstanceResolutionInterfaceProduct Produce()
        {
            var products = new List<ResolutionProduct>();

            var instanceProductDict = SplitByWrapperType();

            foreach (var pair in instanceProductDict)
            {
                var wrapperType = pair.Key;
                var filteredInstanceProduct = pair.Value;

                if (filteredInstanceProduct.Count == 0)
                {
                    continue;
                }

                var wrapperSymbol = BindFrom.GenerateWrapperTypes(_typeInfoProvider, wrapperType);

                var resolutionProduct = CreateResolutionProduct(
                    filteredInstanceProduct,
                    wrapperType,
                    wrapperSymbol
                    );

                products.Add(
                    resolutionProduct
                    );
            }

            var result = new InstanceResolutionInterfaceProduct(
                BindFrom,
                products
                );

            return result;
        }

        private IReadOnlyDictionary<DpdtArgumentWrapperTypeEnum, List<InstanceProduct>> SplitByWrapperType()
        {
            var allWrapperTypes = ArgumentWrapperHelper.GenerateWrapperEnumTypes(true);

            var instanceProductDict = allWrapperTypes
                .ToDictionary(a => a, a => new List<InstanceProduct>())
                ;

            foreach (var instanceProduct in InstanceProducts)
            {
                bool doProduceWrappers = false;
                if (instanceProduct.BindingExtender.BindingContainer.TryGetSettingInScope<WrappersSettings>(out var setting))
                {
                    doProduceWrappers = setting.DoProduceWrappers;
                }

                if (doProduceWrappers)
                {
                    for (var wi = 0; wi < allWrapperTypes.Count; wi++)
                    {
                        var wrapper = allWrapperTypes[wi];
                        instanceProductDict[wrapper].Add(instanceProduct);
                    }
                }
                else
                {
                    instanceProductDict[DpdtArgumentWrapperTypeEnum.None].Add(instanceProduct);
                }
            }

            return instanceProductDict;
        }

        private ResolutionProduct CreateResolutionProduct(
            IReadOnlyList<InstanceProduct> filteredInstanceProducts,
            DpdtArgumentWrapperTypeEnum wrapperType,
            ITypeSymbol wrapperSymbol
            )
        {
            var interfaceProduct = new NamedGeneric1Interface(
                GN.IResolution,
                wrapperSymbol
                );
            var interfaceFastProduct = new NamedGeneric1Interface(
                GN.IResolutionFast,
                wrapperSymbol
                );

            #region get

            var getMethodProduct = CreateGetMethod(
                filteredInstanceProducts, 
                wrapperType, 
                wrapperSymbol
                );

            var getExplicitMethodProduct = CreateExplicitMethod(
                "Get",
                getMethodProduct,
                wrapperType,
                wrapperSymbol
                );

            var canBeImplicitelyCastedToObject = wrapperSymbol.IsReferenceType;

            var nonGenericGetProduct = new CreateTupleProduct(
                new TypeTypePair
                (
                    _typeInfoProvider.SystemType(),
                    wrapperSymbol
                ),
                new TypeStringPair
                (
                    _typeInfoProvider.Func(
                        _typeInfoProvider.GetTypeByMetadataName(typeof(IResolutionRequest).FullName!)!,
                        _typeInfoProvider.Object()!
                        ),
                    canBeImplicitelyCastedToObject
                        ? getMethodProduct.MethodName
                        : $"(r) => {getMethodProduct.MethodName}(r)"
                )
                );

            #endregion

            #region get all

            var getAllMethodProduct = CreateGetAllMethod(
                filteredInstanceProducts,
                wrapperType,
                wrapperSymbol
                );

            var getAllExplicitMethodProduct = CreateExplicitMethod(
                "GetAll",
                getAllMethodProduct,
                wrapperType,
                wrapperSymbol
                );

            var nonGenericGetAllProduct = new CreateTupleProduct(
                new TypeTypePair
                (
                    _typeInfoProvider.SystemType(),
                    wrapperSymbol
                ),
                new TypeStringPair
                (
                    _typeInfoProvider.Func(
                        _typeInfoProvider.GetTypeByMetadataName(typeof(IResolutionRequest).FullName!)!,
                        _typeInfoProvider.Object()!
                        ),
                    canBeImplicitelyCastedToObject
                        ? getAllMethodProduct.MethodName
                        : $"(r) => {getAllMethodProduct.MethodName}(r)"
                )
                );

            #endregion

            #region get fast

            var getFastMethodProduct = MethodProductFactory.Create(
                nameof(IResolutionFast<object>.GetFast),
                new TypeMethodResult(wrapperSymbol),
                (methodName, returnType) =>
                {
                    return $@"
{GN.AggressiveInlining}
public {returnType} {methodName}({returnType} unused)
{{
    return {getMethodProduct.MethodName}(
        null
        );
}}
";
                }
                );


            #endregion

            var resolutionProduct = new ResolutionProduct(
                interfaceProduct,
                interfaceFastProduct,
                getMethodProduct,
                getExplicitMethodProduct,
                nonGenericGetProduct,
                getAllMethodProduct,
                getAllExplicitMethodProduct,
                nonGenericGetAllProduct,
                getFastMethodProduct
                );

            return resolutionProduct;
        }

        private IMethodProduct CreateExplicitMethod(
            string explicitMethodName,
            IMethodProduct nonExplicitMethod,
            DpdtArgumentWrapperTypeEnum wrapperType,
            ITypeSymbol wrapperSymbol
            )
        {
            var explicitMethodProduct = MethodProductFactory.Create(
                explicitMethodName,
                nonExplicitMethod.MethodResult,
                (methodName, returnType) => $@"{returnType} {GN.IResolution}<{wrapperSymbol.ToGlobalDisplayString()}>.{methodName}({GN.IResolutionRequest} resolutionRequest)
{{
    return {nonExplicitMethod.MethodName}(resolutionRequest);
}}
"
                );

            return explicitMethodProduct;
        }


        private IMethodProduct CreateGetAllMethod(
            IReadOnlyList<InstanceProduct> filteredInstanceProducts,
            DpdtArgumentWrapperTypeEnum wrapperType,
            ITypeSymbol wrapperSymbol
            )
        {
            var stableSuffix = GetStableSuffix(filteredInstanceProducts);

            string nonExplicitMethodName =
                $"GetAll_{wrapperSymbol.GetSpecialName()}_{stableSuffix}"
                ;

            var returnType = _typeInfoProvider
                .GetTypeByMetadataName("System.Collections.Generic.List`1")!
                .Construct(wrapperSymbol)
                ;


            var getAllMethodProduct = MethodProductFactory.Create(
                nonExplicitMethodName,
                new TypeMethodResult(returnType),
                (methodName, returnType) =>
                {
                    var methodBody = $@"private {returnType} {methodName}({GN.IResolutionRequest} resolutionRequest)
{{
    resolutionRequest  = resolutionRequest ?? new {GN.ResolutionRequest}<{ClusterBindings.ClusterType.ToGlobalDisplayString()}, {wrapperSymbol.ToGlobalDisplayString()}>(true);

    var result = new global::System.Collections.Generic.List<{wrapperSymbol.ToGlobalDisplayString()}>();
";

                    foreach (var instanceProduct in filteredInstanceProducts)
                    {
                        var modifiedContext = $"target_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()}";

                        if (!(instanceProduct.PredicateMethod is null) || instanceProduct.BindingExtender.NeedToProcessResolutionContext)
                        {
                            methodBody += $@"
    var {modifiedContext} =
        new {GN.ResolutionTarget}<{ClusterBindings.ClusterType.ToGlobalDisplayString()}, {instanceProduct.BindingExtender.BindingContainer.BindToType.ToGlobalDisplayString()}>(
            resolutionRequest
            );
";
                        }

                        if (!(instanceProduct.PredicateMethod is null))
                        {
                            //with predicate (itself is conditional!)

                            methodBody += $@"//predicate method is same for all wrappers, so we does no need for a wrapper-postfix (like _Func)
    if({instanceProduct.PredicateMethod.GetWrappedMethodName(DpdtArgumentWrapperTypeEnum.None)}({modifiedContext}))
    {{
        result.Add(
            {instanceProduct.FactoryObjectMethod.GetWrappedMethodName(wrapperType)}({modifiedContext})
            );
    }}
";
                        }
                        else
                        {
                            //no predicate
                            if (instanceProduct.BindingExtender.NeedToProcessResolutionContext)
                            {
                                //we need a resolution context
                                methodBody += $@"
    result.Add(
        {instanceProduct.FactoryObjectMethod.GetWrappedMethodName(wrapperType)}({modifiedContext})
        );
";
                            }
                            else
                            {
                                //no need for a resolution context
                                methodBody += $@"
    result.Add(
        {instanceProduct.FactoryObjectMethod.GetWrappedMethodName(wrapperType)}(null)
        );
";
                            }

                        }
                    }

                    methodBody += $@"

    return result;
}}
";
                    return methodBody;
                });

            return getAllMethodProduct;
        }


        private IMethodProduct CreateGetMethod(
            IReadOnlyList<InstanceProduct> filteredInstanceProducts, 
            DpdtArgumentWrapperTypeEnum wrapperType,
            ITypeSymbol wrapperSymbol
            )
        {
            var stableSuffix = GetStableSuffix(filteredInstanceProducts);

            var total = filteredInstanceProducts.Count;
            var itselfNonConditional = filteredInstanceProducts.Count(p => !p.BindingExtender.BindingContainer.IsConditional);

            string nonExplicitMethodName = 
                $"Get_{wrapperSymbol.GetSpecialName()}_{stableSuffix}"
                ;

            IMethodProduct getMethodProduct;
            if (total == 0)
            {
                getMethodProduct = MethodProductFactory.Create(
                    nonExplicitMethodName,
                    new TypeMethodResult(wrapperSymbol),
                    (methodName, returnType) => $@"{GN.AggressiveInlining}
private {returnType} {methodName}({GN.IResolutionRequest} resolutionRequest)
{{
    return RaiseNoBindingAvailable<{wrapperSymbol.ToGlobalDisplayString()}>();
}}
");

            }
            else if (total == 1 && itselfNonConditional == 1)
            {
                //only one and non conditional ITSELF
                //we does no need to CheckPredicate for it

                var methodBody = string.Empty;

                var instanceProduct = filteredInstanceProducts[0];
                if (instanceProduct.BindingExtender.NeedToProcessResolutionContext)
                {
                    var modifiedContext = $"target_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()}";

                    methodBody = $@"
    resolutionRequest  = resolutionRequest ?? new {GN.ResolutionRequest}<{ClusterBindings.ClusterType.ToGlobalDisplayString()}, {wrapperSymbol.ToGlobalDisplayString()}>(false);

    var {modifiedContext} =
        new {GN.ResolutionTarget}<{ClusterBindings.ClusterType.ToGlobalDisplayString()}, {instanceProduct.BindingExtender.BindingContainer.BindToType.ToGlobalDisplayString()}>(
            resolutionRequest
            );

    return {filteredInstanceProducts[0].FactoryObjectMethod.GetWrappedMethodName(wrapperType)}({modifiedContext});
";
                }
                else
                {
                    methodBody = $@"
    return {filteredInstanceProducts[0].FactoryObjectMethod.GetWrappedMethodName(wrapperType)}(null);
";
                }

                getMethodProduct = MethodProductFactory.Create(
                    nonExplicitMethodName,
                    new TypeMethodResult(wrapperSymbol),
                    (methodName, returnType) => $@"{GN.AggressiveInlining}
private {returnType} {methodName}({GN.IResolutionRequest} resolutionRequest)
{{
    {methodBody}
}}
");
            }
            else
            {
                if (itselfNonConditional > 1)
                {
                    getMethodProduct = MethodProductFactory.Create(
                        nonExplicitMethodName,
                        new TypeMethodResult(wrapperSymbol),
                        (methodName, returnType) => $@"{GN.AggressiveInlining}
private {returnType} {methodName}({GN.IResolutionRequest} resolutionRequest)
{{
    return RaiseTooManyBindingException<{wrapperSymbol.ToGlobalDisplayString()}>();
}}
");

                }
                else
                {
                    var targetDict = new Dictionary<string, string>();

                    var methodBody = $@"
    resolutionRequest  = resolutionRequest ?? new {GN.ResolutionRequest}<{ClusterBindings.ClusterType.ToGlobalDisplayString()}, {wrapperSymbol.ToGlobalDisplayString()}>(false);

    int allowedChildrenCount = {itselfNonConditional};
";



                    foreach (var instanceProduct in filteredInstanceProducts)
                    {
                        var modifiedContext = $"target_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()}";
                        targetDict[instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()] = modifiedContext;

                        methodBody += $@"
    var predicate_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()} = false;
    var {modifiedContext} =
        new {GN.ResolutionTarget}<{ClusterBindings.ClusterType.ToGlobalDisplayString()}, {instanceProduct.BindingExtender.BindingContainer.BindToType.ToGlobalDisplayString()}>(
            resolutionRequest
            );
";

                        if (!(instanceProduct.PredicateMethod is null))
                        {
                            methodBody += $@"//predicate method is same for all wrappers, so we does no need for a wrapper-postfix (like _Func)
    if({instanceProduct.PredicateMethod.GetWrappedMethodName(DpdtArgumentWrapperTypeEnum.None)}({modifiedContext}))
    {{
        if(++allowedChildrenCount > 1)
        {{
            RaiseTooManyBindingException<{wrapperSymbol.ToGlobalDisplayString()}>();
        }}

        predicate_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()} = true;
    }}
";
                        }
                    }

                    methodBody += $@"
    if(allowedChildrenCount == 0)
    {{
        return RaiseNoBindingAvailable<{wrapperSymbol.ToGlobalDisplayString()}>();
    }}
";

                    for (var ipIndex = 0; ipIndex < filteredInstanceProducts.Count; ipIndex++)
                    {
                        var instanceProduct = filteredInstanceProducts[ipIndex];
                        var isLast = ipIndex == filteredInstanceProducts.Count - 1;

                        var modifiedContext =
                            targetDict[instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()];

                        if (instanceProduct.BindingExtender.NeedToProcessResolutionContext && !isLast)
                        {
                            methodBody += $@"
    if(predicate_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()})
    {{
        return {instanceProduct.FactoryObjectMethod.GetWrappedMethodName(wrapperType)}({modifiedContext});
    }}
";
                        }
                        else
                        {
                            methodBody += $@"
    return {instanceProduct.FactoryObjectMethod.GetWrappedMethodName(wrapperType)}({modifiedContext});
";
                        }
                    }

                    getMethodProduct = MethodProductFactory.Create(
                        nonExplicitMethodName,
                        new TypeMethodResult(wrapperSymbol),
                        (methodName, returnType) => $@"{GN.AggressiveInlining}
private {returnType} {methodName}({GN.IResolutionRequest} resolutionRequest)
{{
    {methodBody}
}}
");

                }
            }

            return getMethodProduct;
        }



        private static string GetStableSuffix(
            IReadOnlyList<InstanceProduct> filteredInstanceProducts
            )
        {
            if (filteredInstanceProducts is null)
            {
                throw new ArgumentNullException(nameof(filteredInstanceProducts));
            }

            var suffix = string.Join(
                "",
                filteredInstanceProducts.ConvertAll(fip => fip.BindingExtender.BindingContainer.GetStableSuffix())
                ).GetStringSha256Hash().SafeSubstring(0, 16);

            return suffix;
        }

    }
}