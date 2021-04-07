using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer.Product;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Core.Producer.ClassProducer.Proxy
{
    public class ProxiedMemberFactory : IProxyMemberFactory
    {
        private static readonly string _proxySnippet = @$"
                var sessionGuid = _sessionSaver.{nameof(BaseSessionSaver.StartSessionSafely)}(
                    _payloadFullName,
                    <membername>,
                    <arguments>
                    );

                var startDate = global::System.Diagnostics.Stopwatch.GetTimestamp();
                try
                {{
                    <doing>

                    _sessionSaver.{nameof(BaseSessionSaver.FixSessionSafely)}(
                        sessionGuid,
                        (global::System.Diagnostics.Stopwatch.GetTimestamp() - startDate) / _stopwatchFrequency,
                        null
                        );

                    <returning>
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
";

        public EventProduct GetEventProduct(
            IEventSymbol @event
            )
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            //add and remove method shares same arguments
            var proxyArguments = GetProxyArguments(@event.AddMethod ?? @event.RemoveMethod!);


            return new EventProduct(
                "public",
                @event,
                (@event.AddMethod != null
                    ? GetProxySnippet(
                        $"nameof({@event.Name})",
                        proxyArguments,
                        $"_payload.{@event.Name} += value;",
                        string.Empty
                        )
                    : null
                    ),
                (@event.RemoveMethod != null
                    ? GetProxySnippet(
                        $"nameof({@event.Name})",
                        proxyArguments,
                        $"_payload.{@event.Name} -= value;",
                        string.Empty
                        )
                    : null
                    )
                );
        }

        public IndexerProduct GetIndexerProduct(
            IPropertySymbol indexer
            )
        {
            if (indexer is null)
            {
                throw new ArgumentNullException(nameof(indexer));
            }

            var parameters = indexer.GetJoinedParametersName();

            //get and set method shares same arguments
            var proxyArguments = GetProxyArguments(indexer.GetMethod ?? indexer.SetMethod!);

            return new IndexerProduct(
                "public",
                indexer,
                (indexer.GetMethod != null
                    ? GetProxySnippet(
                        $"\"{indexer.Type.ToFullDisplayString()} {indexer.Name}\"",
                        proxyArguments,
                        $"var result = _payload[{parameters}];",
                        "return result;"
                        )
                    : null
                    ),
                (indexer.SetMethod != null
                    ? GetProxySnippet(
                        $"\"{indexer.Type.ToFullDisplayString()} {indexer.Name}\"",
                        proxyArguments,
                        $"_payload[{parameters}] = value;",
                        string.Empty
                        )
                    : null
                    )
                );
        }

        public PropertyProduct GetPropertyProduct(
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
                (property.GetMethod != null
                    ? GetProxySnippet($"nameof({property.Name})", "null", $"var result = _payload.{property.Name};", "return result;")
                    : null
                    ),
                (property.SetMethod != null
                    ? GetProxySnippet($"nameof({property.Name})", "null", $"_payload.{property.Name} = value;", string.Empty)
                    : null
                    )
                );
        }

        public IMethodProduct GetMethodProduct(
            IMethodSymbol method
            )
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var extractor = new MethodArgumentExtractor();
            var arguments = extractor.GetMethodArguments(method);

            var proxyArguments = GetProxyArguments(method);

            IMethodProduct result;
            if (method.ReturnsVoid)
            {
                result = MethodProductFactory.Create(
                    method,
                    arguments,
                    (methodName, h) =>
                    {
                        var codeSnippet = GetProxySnippet(
                            $"nameof({methodName})",
                            proxyArguments,
                            $"_payload.{methodName}({arguments.Join(cafm => cafm.GetUsageSyntax(), ", ")});",
                            string.Empty
                            );

                        return $@"public {h}
        {{
{codeSnippet}
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
                    arguments,
                    (methodName, h) =>
                    {
                        var codeSnippet = GetProxySnippet(
                            $"nameof({methodName})",
                            proxyArguments,
                            $"var result = _payload.{methodName}({arguments.Join(cafm => cafm.GetUsageSyntax(), ", ")});",
                            "return result;"
                            );

                        return $@"public {h}
        {{
{codeSnippet}
        }}
";
                    }
                    );
            }

            return result;
        }

        private string GetProxySnippet(
            string memberName,
            string arguments,
            string doing,
            string returning
            )
        {
            return _proxySnippet
                .Replace("<membername>", memberName)
                .Replace("<arguments>", arguments)
                .Replace("<doing>", doing)
                .Replace("<returning>", returning)
                ;
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
                if (parameter.RefKind == RefKind.Out)
                {
                    continue;
                }

                sb.Add($"nameof({parameter.Name})");
                sb.Add($"{parameter.Name}");
            }

            if (sb.Count == 0)
            {
                return "null";
            }

            var joined = string.Join(",", sb);

            return $"new object[] {{ {joined} }}";
        }

    }
}
