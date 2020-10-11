using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Product;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace DpdtInject.Generator.Producer
{
    internal class BindFromResolutionProducer
    {
        private readonly Compilation _compilation;

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
            Compilation compilation,
            ClusterBindings clusterBindings,
            ITypeSymbol bindFrom,
            IReadOnlyList<InstanceProduct> instanceProducts
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

            if (bindFrom is null)
            {
                throw new System.ArgumentNullException(nameof(bindFrom));
            }

            if (instanceProducts is null)
            {
                throw new ArgumentNullException(nameof(instanceProducts));
            }
            _compilation = compilation;
            ClusterBindings = clusterBindings;
            BindFrom = bindFrom;
            InstanceProducts = instanceProducts;
        }


        public InstanceResolutionInterfaceProduct Produce()
        {
            var products = new List<ResolutionProduct>();

            var filteredInstanceProducts = InstanceProducts.FindAll(ip => ip.BindingExtender.BindingContainer.IsRegisteredFrom(BindFrom));

            foreach (var (wrapperType, wrapperSymbol) in BindFrom.GenerateWrapperTypes(_compilation, true))
            {
                var resolutionProduct = CreateResolutionProduct(
                    filteredInstanceProducts,
                    wrapperType,
                    wrapperSymbol
                    );

                products.Add(
                    resolutionProduct
                    );
            }

            var result = new InstanceResolutionInterfaceProduct(
                products
                );

            return result;
        }

        private ResolutionProduct CreateResolutionProduct(
            IReadOnlyList<InstanceProduct> filteredInstanceProducts,
            DpdtArgumentWrapperTypeEnum wrapperType,
            ITypeSymbol wrapperSymbol
            )
        {
            var interfaceProduct = new InterfaceProduct(
                $"{nameof(IResolution<object>)}<{wrapperSymbol.ToDisplayString()}>"
                );
            var interfaceFastProduct = new InterfaceProduct(
                $"{nameof(IResolutionFast<object>)}<{wrapperSymbol.ToDisplayString()}>"
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

            var nonGenericGetProduct = new CreateTupleProduct(
                (
                    _compilation.SystemType(),
                    $"typeof({wrapperSymbol.ToDisplayString()})"
                ),
                (
                    _compilation.Func(
                        _compilation.GetTypeByMetadataName(typeof(IResolutionRequest).FullName!)!,
                        _compilation.Object()!
                        ),
                    getMethodProduct.MethodName
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
                (
                    _compilation.SystemType(),
                    $"typeof({wrapperSymbol.ToDisplayString()})"
                ),
                (
                    _compilation.Func(
                        _compilation.GetTypeByMetadataName(typeof(IResolutionRequest).FullName!)!,
                        _compilation.Object()!
                        ),
                    getAllMethodProduct.MethodName
                )
                );

            #endregion

            #region get fast

            var getFastMethodProduct = new MethodProduct(
                nameof(IResolutionFast<object>.GetFast),
                wrapperSymbol,
                (methodName, returnType) =>
                {
                    return $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public {returnType.ToDisplayString()} {methodName}({returnType.ToDisplayString()} unused)
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

        private MethodProduct CreateExplicitMethod(
            string explicitMethodName,
            MethodProduct nonExplicitMethod,
            DpdtArgumentWrapperTypeEnum wrapperType,
            ITypeSymbol wrapperSymbol
            )
        {
            var explicitMethodProduct = new MethodProduct(
                explicitMethodName,
                nonExplicitMethod.ReturnType,
                (methodName, returnType) => $@"
{returnType.ToDisplayString()} IResolution<{wrapperSymbol.ToDisplayString()}>.{methodName}(IResolutionRequest resolutionRequest)
{{
    return {nonExplicitMethod.MethodName}(resolutionRequest);
}}
"
                );

            return explicitMethodProduct;
        }


        private MethodProduct CreateGetAllMethod(
            IReadOnlyList<InstanceProduct> filteredInstanceProducts,
            DpdtArgumentWrapperTypeEnum wrapperType,
            ITypeSymbol wrapperSymbol
            )
        {
            string nonExplicitMethodName =
                $"GetAll_{wrapperSymbol.ToDisplayString().EscapeSpecialTypeSymbols()}_{Guid.NewGuid().RemoveMinuses()}"
                ;

            var returnType = _compilation
                .GetTypeByMetadataName("System.Collections.Generic.List`1")!
                .Construct(wrapperSymbol)
                ;

            MethodProduct getAllMethodProduct = new MethodProduct(
                nonExplicitMethodName,
                returnType,
                (methodName, returnType) =>
                {
                    var methodBody = $@"
private {returnType.ToDisplayString()} {methodName}(IResolutionRequest resolutionRequest)
{{
    resolutionRequest  = resolutionRequest ?? new {nameof(ResolutionRequest<object, object>)}<{ClusterBindings.ClusterType.ToDisplayString()}, {wrapperSymbol.ToDisplayString()}>(true);

    var result = new List<{wrapperSymbol.ToDisplayString()}>();
";

                    foreach (var instanceProduct in filteredInstanceProducts)
                    {
                        var modifiedContext = $"target_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()}";

                        if (!(instanceProduct.PredicateMethod is null) || instanceProduct.BindingExtender.NeedToProcessResolutionContext)
                        {
                            methodBody += $@"
var {modifiedContext} =
    new {nameof(ResolutionTarget<object, object>)}<{ClusterBindings.ClusterType.ToDisplayString()}, {instanceProduct.BindingExtender.BindingContainer.BindToType.ToDisplayString()}>(
        resolutionRequest
        );
";
                        }

                        if (!(instanceProduct.PredicateMethod is null))
                        {
                            //with predicate (itself is conditional!)

                            methodBody += $@"
//predicate method is same for all wrappers, so we does no need for a wrapper-postfix (like _Func)
if({instanceProduct.PredicateMethod.GetMethodName(DpdtArgumentWrapperTypeEnum.None)}({modifiedContext}))
{{
    result.Add(
        {instanceProduct.FactoryObjectMethod.GetMethodName(wrapperType)}({modifiedContext})
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
    {instanceProduct.FactoryObjectMethod.GetMethodName(wrapperType)}({modifiedContext})
    );
";
                            }
                            else
                            {
                                //no need for a resolution context
                                methodBody += $@"
result.Add(
    {instanceProduct.FactoryObjectMethod.GetMethodName(wrapperType)}(null)
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


        private MethodProduct CreateGetMethod(
            IReadOnlyList<InstanceProduct> filteredInstanceProducts, 
            DpdtArgumentWrapperTypeEnum wrapperType, 
            ITypeSymbol wrapperSymbol
            )
        {
            var total = filteredInstanceProducts.Count;
            var itselfNonConditional = filteredInstanceProducts.Count(p => !p.BindingExtender.BindingContainer.IsConditional);

            string nonExplicitMethodName = 
                $"Get_{wrapperSymbol.ToDisplayString().EscapeSpecialTypeSymbols()}_{Guid.NewGuid().RemoveMinuses()}"
                ;

            MethodProduct getMethodProduct;
            if (total == 0)
            {
                getMethodProduct = new MethodProduct(
                    nonExplicitMethodName,
                    wrapperSymbol,
                    (methodName, returnType) => $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(IResolutionRequest resolutionRequest)
{{
    return RaiseNoBindingAvailable<{wrapperSymbol.ToDisplayString()}>();
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
resolutionRequest  = resolutionRequest ?? new {nameof(ResolutionRequest<object, object>)}<{ClusterBindings.ClusterType.ToDisplayString()}, {wrapperSymbol.ToDisplayString()}>(false);

var {modifiedContext} =
    new {nameof(ResolutionTarget<object, object>)}<{ClusterBindings.ClusterType.ToDisplayString()}, {instanceProduct.BindingExtender.BindingContainer.BindToType.ToDisplayString()}>(
        resolutionRequest
        );

return {filteredInstanceProducts[0].FactoryObjectMethod.GetMethodName(wrapperType)}({modifiedContext});
";
                }
                else
                {
                    methodBody = $@"
return {filteredInstanceProducts[0].FactoryObjectMethod.GetMethodName(wrapperType)}(null);
";
                }

                getMethodProduct = new MethodProduct(
                    nonExplicitMethodName,
                    wrapperSymbol,
                    (methodName, returnType) => $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(IResolutionRequest resolutionRequest)
{{
    {methodBody}
}}
");

            }
            else
            {
                if (itselfNonConditional > 1)
                {
                    getMethodProduct = new MethodProduct(
                        nonExplicitMethodName,
                        wrapperSymbol,
                        (methodName, returnType) => $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(IResolutionRequest resolutionRequest)
{{
    return RaiseTooManyBindingException<{wrapperSymbol.ToDisplayString()}>();
}}
");

                }
                else
                {
                    var targetDict = new Dictionary<string, string>();

                    var methodBody = $@"
resolutionRequest  = resolutionRequest ?? new {nameof(ResolutionRequest<object, object>)}<{ClusterBindings.ClusterType.ToDisplayString()}, {wrapperSymbol.ToDisplayString()}>(false);

int allowedChildrenCount = {itselfNonConditional};
";



                    foreach (var instanceProduct in filteredInstanceProducts)
                    {
                        var modifiedContext = $"target_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()}";
                        targetDict[instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()] = modifiedContext;

                        methodBody += $@"
var predicate_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()} = false;
var {modifiedContext} =
    new {nameof(ResolutionTarget<object, object>)}<{ClusterBindings.ClusterType.ToDisplayString()}, {instanceProduct.BindingExtender.BindingContainer.BindToType.ToDisplayString()}>(
        resolutionRequest
        );
";

                        if (!(instanceProduct.PredicateMethod is null))
                        {
                            methodBody += $@"
//predicate method is same for all wrappers, so we does no need for a wrapper-postfix (like _Func)
if({instanceProduct.PredicateMethod.GetMethodName(DpdtArgumentWrapperTypeEnum.None)}({modifiedContext}))
{{
    if(++allowedChildrenCount > 1)
    {{
        RaiseTooManyBindingException<{wrapperSymbol.ToDisplayString()}>();
    }}

    predicate_{instanceProduct.BindingExtender.BindingContainer.GetStableSuffix()} = true;
}}
";
                        }
                    }

                    methodBody += $@"
if(allowedChildrenCount == 0)
{{
    return RaiseNoBindingAvailable<{wrapperSymbol.ToDisplayString()}>();
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
    return {instanceProduct.FactoryObjectMethod.GetMethodName(wrapperType)}({modifiedContext});
}}
";
                        }
                        else
                        {
                            methodBody += $@"
return {instanceProduct.FactoryObjectMethod.GetMethodName(wrapperType)}({modifiedContext});
";
                        }
                    }

                    getMethodProduct = new MethodProduct(
                        nonExplicitMethodName,
                        wrapperSymbol,
                        (methodName, returnType) => $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private {returnType.ToDisplayString()} {methodName}(IResolutionRequest resolutionRequest)
{{
    {methodBody}
}}
");

                }
            }

            return getMethodProduct;
        }
    }

}