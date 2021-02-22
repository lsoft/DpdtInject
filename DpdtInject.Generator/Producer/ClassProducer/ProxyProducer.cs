using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DpdtInject.Generator.BindExtractor;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Producer.Product;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.ClassProducer
{
    internal class ProxyProducer : IClassProducer
    {
        private readonly BindingContainerTypes _types;
        private readonly ITypeSymbol _methodAttributeType;
        private readonly ITypeSymbol _sessionSaverType;

        public ProxyProducer(
            BindingContainerTypes types,
            ITypeSymbol methodAttributeType,
            ITypeSymbol sessionSaverType
            )
        {
            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (methodAttributeType is null)
            {
                throw new ArgumentNullException(nameof(methodAttributeType));
            }

            if (sessionSaverType is null)
            {
                throw new ArgumentNullException(nameof(sessionSaverType));
            }

            _types = types;
            _methodAttributeType = methodAttributeType;
            _sessionSaverType = sessionSaverType;
        }

        /// <inheritdoc />
        public ProducedClassProduct GenerateProduct()
        {
            var methodProducts = ScanForMethodsToImplement(
                );

            var compilationUnit = DpdtInject.Generator.Properties.Resource.CarcassProxy;

            var fixedCompilationUnit = compilationUnit
                .CheckAndReplace(
                    "//PROXYPRODUCER: aggressive inline and optimize",
                    "[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]"
                    )
                .ReplaceLineStartsWith(
                    "namespace",
                    $"namespace {_types.BindToType.ContainingNamespace.ToDisplayString()}"
                    )
                .ReplaceLineStartsWith(
                    "using BindFromType",
                    $""
                    )
                .ReplaceLineStartsWith(
                    "using SessionSaver",
                    $""
                    )
                .CheckAndReplace(
                    nameof(CarcassProxy),
                    _types.BindToType.Name
                    )
                .CheckAndReplace(
                    "BindFromType",
                    _types.BindFromTypes[0].ToDisplayString()
                    )
                .CheckAndReplace(
                    "SessionSaver",
                    _sessionSaverType.ToDisplayString()
                    )
                .CheckAndReplace(
                    "//PROXYPRODUCER: put methods here",
                    methodProducts.Join(mp => mp.MethodBody)
                    )
                ;

            var result = new ProducedClassProduct(
                _types.BindToType,
                fixedCompilationUnit
                );

            return result;
        }


        private List<MethodProduct> ScanForMethodsToImplement(
            )
        {
            var result = new List<MethodProduct>();
            var declaredMethods = _types.BindFromTypes[0].GetMembers().FindAll(m => m.Kind == SymbolKind.Method);

            foreach (IMethodSymbol declaredMethod in declaredMethods)
            {
                var implementedMethod = _types.BindToType.FindImplementationForInterfaceMember(declaredMethod);
                if (!(implementedMethod is null))
                {
                    continue;
                }

                //method is not implemented in the proto class

                var telemetryAttribute = declaredMethod
                    .GetAttributes()
                    .FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, _methodAttributeType))
                    ;

                if (telemetryAttribute is not null)
                {
                    var declaredMethodProduct = GetProxiedMethodProduct(
                        declaredMethod
                        );

                    result.Add(declaredMethodProduct);
                }
                else
                {
                    var declaredMethodProduct = GetNonProxiedMethodProduct(
                        declaredMethod
                        );

                    result.Add(declaredMethodProduct);
                }
            }

            return result;
        }

        private MethodProduct GetNonProxiedMethodProduct(
            IMethodSymbol methodSymbol
            )
        {
            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            var extractor = new ConstructorArgumentFromMethodExtractor();
            var constructorArguments = extractor.GetConstructorArguments(methodSymbol);

            MethodProduct result;
            if (methodSymbol.ReturnType.ToDisplayString() == "void")
            {
                result = new MethodProduct(
                    methodSymbol.Name,
                    methodSymbol.ReturnType,
                    (
                        methodName,
                        returnType
                        ) =>
                    {
                        return $@"
        public {returnType.ToDisplayString()} {methodName}({constructorArguments.Join(ca => ca.GetDeclarationSyntax(), ",")})
        {{
            _payload.{methodName}({constructorArguments.Join(ca => ca.GetUsageSyntax(), ",")});
        }}
";
                    }
                    );
            }
            else
            {
                result = new MethodProduct(
                    methodSymbol.Name,
                    methodSymbol.ReturnType,
                    (
                        methodName,
                        returnType
                        ) =>
                    {
                        return $@"
        public {returnType.ToDisplayString()} {methodName}({constructorArguments.Join(ca => ca.GetDeclarationSyntax(), ",")})
        {{
            return _payload.{methodName}({constructorArguments.Join(ca => ca.GetUsageSyntax(), ",")});
        }}
";
                    }
                    );
            }


            return result;
        }

        private MethodProduct GetProxiedMethodProduct(
            IMethodSymbol methodSymbol
            )
        {
            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            var extractor = new ConstructorArgumentFromMethodExtractor();
            var constructorArguments = extractor.GetConstructorArguments(methodSymbol);

            MethodProduct result;
            if (methodSymbol.ReturnType.ToDisplayString() == "void")
            {
                result = new MethodProduct(
                    methodSymbol.Name,
                    methodSymbol.ReturnType,
                    (
                        methodName,
                        returnType
                        ) =>
                    {
                        return $@"
        public {returnType.ToDisplayString()} {methodName}({constructorArguments.Join(cafm => cafm.GetDeclarationSyntax(), ",")})
        {{
            Invoke(
                () => _payload.{methodName}({constructorArguments.Join(cafm => cafm.GetUsageSyntax(), ",")}),
                nameof({methodName}),
                {GetProxyArguments(methodSymbol)}
                );
        }}
";
                    }
                    );
            }
            else
            {
                result = new MethodProduct(
                    methodSymbol.Name,
                    methodSymbol.ReturnType,
                    (
                        methodName,
                        returnType
                        ) =>
                    {
                        return $@"
        public {returnType.ToDisplayString()} {methodName}({constructorArguments.Join(cafm => cafm.GetDeclarationSyntax(), ",")})
        {{
            var result = default({returnType.ToDisplayString()});

            Invoke(
                () => result = _payload.{methodName}({constructorArguments.Join(cafm => cafm.GetUsageSyntax(), ",")}),
                nameof({methodName}),
                {GetProxyArguments(methodSymbol)}
                );

            return result!;
        }}
";
                    }
                    );
            }


            return result;
        }

        private string GetProxyArguments(
            IMethodSymbol methodSymbol
            )
        {
            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            if (methodSymbol.Parameters.Length == 0)
            {
                return "null";
            }

            var sb = new List<string>();

            foreach (var parameter in methodSymbol.Parameters)
            {
                sb.Add($"nameof({parameter.Name})");
                sb.Add($"{parameter.Name}");
            }

            var joined = string.Join(",", sb);

            return $"new object[] {{ {joined} }}";
        }

    }
}
