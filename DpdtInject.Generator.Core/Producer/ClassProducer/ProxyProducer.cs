using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Producer.ClassProducer.Product;
using DpdtInject.Generator.Core.Producer.Product;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Producer.ClassProducer
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
        public IProducedClassProduct GenerateProduct()
        {
            var methodProducts = ScanForMethodsToImplement(
                );

            var result = new ProxyClassProduct(
                _types.BindFromTypes[0],
                _types.BindToType,
                _sessionSaverType,
                methodProducts
                );

            return result;
        }


        private List<IMethodProduct> ScanForMethodsToImplement(
            )
        {
            var result = new List<IMethodProduct>();
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

        private IMethodProduct GetNonProxiedMethodProduct(
            IMethodSymbol methodSymbol
            )
        {
            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            var extractor = new ConstructorArgumentFromMethodExtractor();
            var constructorArguments = extractor.GetConstructorArguments(methodSymbol);


            var returnModifier = methodSymbol.ReturnsVoid
                ? string.Empty
                : "return"
                ;

            var refModifier =
                (methodSymbol.ReturnsByRef || methodSymbol.ReturnsByRefReadonly)
                    ? "ref"
                    : string.Empty
                ;

            var result = MethodProductFactory.Create(
                methodSymbol,
                constructorArguments,
                (methodName, h) =>
                {
                    return $@"public {h}
        {{
            {returnModifier} {refModifier} _payload.{methodName}({constructorArguments.Join(ca => ca.GetUsageSyntax(), ",")});
        }}
";
                }
                );
            return result;
        }

        private IMethodProduct GetProxiedMethodProduct(
            IMethodSymbol methodSymbol
            )
        {
            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            var extractor = new ConstructorArgumentFromMethodExtractor();
            var constructorArguments = extractor.GetConstructorArguments(methodSymbol);

            var proxyArguments = GetProxyArguments(methodSymbol);

            IMethodProduct result;
            if (methodSymbol.ReturnsVoid)
            {
                result = MethodProductFactory.Create(
                    methodSymbol,
                    constructorArguments,
                    (methodName, h) =>
                    {
                        return $@"public {h}
        {{
            var sessionGuid = _sessionSaver.{nameof(BaseSessionSaver.StartSessionSafely)}(
                _payloadFullName,
                nameof({methodName}),
                {proxyArguments}
                );

            var startDate = System.Diagnostics.Stopwatch.GetTimestamp();
            try
            {{
                _payload.{methodName}({constructorArguments.Join(cafm => cafm.GetUsageSyntax(), ",")});

                _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                    sessionGuid,
                    (System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                    null
                    );
            }}
            catch (Exception excp)
            {{
                _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                    sessionGuid,
                    (System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                    excp
                    );

                throw;
            }}
        }}
";
                    }
                    );
            }
            else
            {
                var refModifier = 
                    (methodSymbol.ReturnsByRef || methodSymbol.ReturnsByRefReadonly)
                        ? "ref"
                        : string.Empty
                    ;

                result = MethodProductFactory.Create(
                    methodSymbol,
                    constructorArguments,
                    (methodName, h) =>
                    {
                        return $@"public {h}
        {{
            var sessionGuid = _sessionSaver.{nameof(BaseSessionSaver.StartSessionSafely)}(
                _payloadFullName,
                nameof({methodName}),
                {proxyArguments}
                );

            var startDate = System.Diagnostics.Stopwatch.GetTimestamp();
            try
            {{
                var result = {refModifier} _payload.{methodName}({constructorArguments.Join(cafm => cafm.GetUsageSyntax(), ",")});

                _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                    sessionGuid,
                    (System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                    null
                    );

                return result;
            }}
            catch (Exception excp)
            {{
                _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                    sessionGuid,
                    (System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                    excp
                    );

                throw;
            }}
        }}";
                    }
                    );
            }


            return result;
        }

        private static string GetProxyArguments(
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
