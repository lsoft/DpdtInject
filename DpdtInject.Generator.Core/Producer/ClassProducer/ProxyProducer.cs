using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Helpers;
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
            var methodProducts = ScanForMembersToImplement(
                );

            var result = new ProxyClassProduct(
                _types.BindFromTypes[0],
                _types.BindToType,
                _sessionSaverType,
                methodProducts
                );

            return result;
        }


        private List<IWritable> ScanForMembersToImplement(
            )
        {
            var result = new List<IWritable>();

            var members = (
                from m in _types.BindFromTypes[0].GetMembers()
                where
                    (m.Kind == SymbolKind.Method && (m is IMethodSymbol mms) && mms.MethodKind == MethodKind.Ordinary)
                    ||
                    (m.Kind == SymbolKind.Property)
                select m
                ).ToList();

            foreach (var member in members)
            {
                var telemetryAttribute = member
                    .GetAttributes()
                    .FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, _methodAttributeType))
                    ;

                var implementedMember = _types.BindToType.FindImplementationForInterfaceMember(member);
                if (!(implementedMember is null))
                {
                    //member already implemented into proto class

                    if (telemetryAttribute is not null)
                    {
                        //but it marked with proxy attribute!

                        throw new DpdtException(
                            DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                            $"Proxy binding for [{_types.BindToType.ToGlobalDisplayString()}] has proxied member [{member.Name}] which is implemented into proto class. Remove implementation or proxy attribute.",
                            _types.BindToType.ToGlobalDisplayString()
                            );
                    }

                    continue;
                }

                //member is not implemented in the proto class

                if (member is IPropertySymbol property && !property.IsIndexer)
                {
                    if (telemetryAttribute is not null)
                    {
                        var declaredPropertyProduct = GetProxiedPropertyProduct(
                            property
                            );

                        result.Add(declaredPropertyProduct);
                    }
                    else
                    {
                        var declaredPropertyProduct = GetNonProxiedPropertyProduct(
                            property
                            );

                        result.Add(declaredPropertyProduct);
                    }
                }
                if (member is IPropertySymbol indexer && indexer.IsIndexer)
                {
                    if (telemetryAttribute is not null)
                    {
                        var declaredPropertyProduct = GetProxiedIndexerProduct(
                            indexer
                            );

                        result.Add(declaredPropertyProduct);
                    }
                    else
                    {
                        var declaredPropertyProduct = GetNonProxiedIndexerProduct(
                            indexer
                            );

                        result.Add(declaredPropertyProduct);
                    }
                }
                if (member is IMethodSymbol method)
                {
                    if (telemetryAttribute is not null)
                    {
                        var declaredMethodProduct = GetProxiedMethodProduct(
                            method
                            );

                        result.Add(declaredMethodProduct);
                    }
                    else
                    {
                        var declaredMethodProduct = GetNonProxiedMethodProduct(
                            method
                            );

                        result.Add(declaredMethodProduct);
                    }
                }
            }

            return result;
        }



        private IndexerProduct GetProxiedIndexerProduct(
            IPropertySymbol indexer
            )
        {
            if (indexer is null)
            {
                throw new ArgumentNullException(nameof(indexer));
            }

            var parameters = indexer.GetJoinedParametersName();

            return new IndexerProduct(
                "public",
                indexer,
                (indexer.GetMethod != null ? $@"
                var sessionGuid = _sessionSaver.{nameof(BaseSessionSaver.StartSessionSafely)}(
                    _payloadFullName,
                    ""{indexer.Type.ToFullDisplayString()} {indexer.Name}"",
                    null
                    );

                var startDate = global::System.Diagnostics.Stopwatch.GetTimestamp();
                try
                {{
                    var result = _payload[{parameters}];

                    _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                        sessionGuid,
                        (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                        null
                        );

                    return result;
                }}
                catch (global::System.Exception excp)
                {{
                    _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                        sessionGuid,
                        (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                        excp
                        );

                    throw;
                }}
" : null),
                (indexer.SetMethod != null ? $@"
                var sessionGuid = _sessionSaver.{nameof(BaseSessionSaver.StartSessionSafely)}(
                    _payloadFullName,
                    ""{indexer.Type.ToFullDisplayString()} {indexer.Name}"",
                    null
                    );

                var startDate = global::System.Diagnostics.Stopwatch.GetTimestamp();
                try
                {{
                    _payload[{parameters}] = value;

                    _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                        sessionGuid,
                        (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                        null
                        );
                }}
                catch (global::System.Exception excp)
                {{
                    _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                        sessionGuid,
                        (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                        excp
                        );

                    throw;
                }}
" : null)
                );
        }

        private IndexerProduct GetNonProxiedIndexerProduct(
            IPropertySymbol indexer
            )
        {
            if (indexer is null)
            {
                throw new ArgumentNullException(nameof(indexer));
            }

            var parameters = indexer.GetJoinedParametersName();

            return new IndexerProduct(
                "public",
                indexer,
                (indexer.GetMethod != null ? $"return _payload[{parameters}];" : null),
                (indexer.SetMethod != null ? $"_payload[{parameters}] = value;" : null)
                );
        }


        private PropertyProduct GetProxiedPropertyProduct(
            IPropertySymbol property
            )
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return new PropertyProduct(
                "public",
                property,
                (property.GetMethod != null ? $@"
                var sessionGuid = _sessionSaver.{nameof(BaseSessionSaver.StartSessionSafely)}(
                    _payloadFullName,
                    nameof({property.Name}),
                    null
                    );

                var startDate = global::System.Diagnostics.Stopwatch.GetTimestamp();
                try
                {{
                    var result = _payload.{property.Name};

                    _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                        sessionGuid,
                        (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                        null
                        );

                    return result;
                }}
                catch (global::System.Exception excp)
                {{
                    _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                        sessionGuid,
                        (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                        excp
                        );

                    throw;
                }}
" : null),
                (property.SetMethod != null ? $@"
                var sessionGuid = _sessionSaver.{nameof(BaseSessionSaver.StartSessionSafely)}(
                    _payloadFullName,
                    nameof({property.Name}),
                    null
                    );

                var startDate = global::System.Diagnostics.Stopwatch.GetTimestamp();
                try
                {{
                    _payload.{property.Name} = value;

                    _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                        sessionGuid,
                        (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                        null
                        );
                }}
                catch (global::System.Exception excp)
                {{
                    _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                        sessionGuid,
                        (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                        excp
                        );

                    throw;
                }}
" : null)
                );
        }

        private PropertyProduct GetNonProxiedPropertyProduct(
            IPropertySymbol property
            )
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return new PropertyProduct(
                "public",
                property,
                (property.GetMethod != null ? $"return _payload.{property.Name};" : null),
                (property.SetMethod != null ? $"_payload.{property.Name} = value;" : null)
                );
        }


        private IMethodProduct GetNonProxiedMethodProduct(
            IMethodSymbol method
            )
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var extractor = new ConstructorArgumentFromMethodExtractor();
            var constructorArguments = extractor.GetConstructorArguments(method);


            var returnModifier = method.ReturnsVoid
                ? string.Empty
                : "return"
                ;

            var refModifier =
                (method.ReturnsByRef || method.ReturnsByRefReadonly)
                    ? "ref"
                    : string.Empty
                ;

            var result = MethodProductFactory.Create(
                method,
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
            IMethodSymbol method
            )
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var extractor = new ConstructorArgumentFromMethodExtractor();
            var constructorArguments = extractor.GetConstructorArguments(method);

            var proxyArguments = GetProxyArguments(method);

            IMethodProduct result;
            if (method.ReturnsVoid)
            {
                result = MethodProductFactory.Create(
                    method,
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

            var startDate = global::System.Diagnostics.Stopwatch.GetTimestamp();
            try
            {{
                _payload.{methodName}({constructorArguments.Join(cafm => cafm.GetUsageSyntax(), ",")});

                _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                    sessionGuid,
                    (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                    null
                    );
            }}
            catch (global::System.Exception excp)
            {{
                _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                    sessionGuid,
                    (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
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
                    (method.ReturnsByRef || method.ReturnsByRefReadonly)
                        ? "ref"
                        : string.Empty
                    ;

                result = MethodProductFactory.Create(
                    method,
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

            var startDate = global::System.Diagnostics.Stopwatch.GetTimestamp();
            try
            {{
                var result = {refModifier} _payload.{methodName}({constructorArguments.Join(cafm => cafm.GetUsageSyntax(), ",")});

                _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                    sessionGuid,
                    (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                    null
                    );

                return result;
            }}
            catch (global::System.Exception excp)
            {{
                _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                    sessionGuid,
                    (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
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
