using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer.Product;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace DpdtInject.Generator.Core.Producer.ClassProducer.Proxy
{

    public class NotProxiedMemberFactory : IProxyMemberFactory
    {
        public EventProduct GetEventProduct(
            IEventSymbol @event
            )
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            return new EventProduct(
                "public",
                @event,
                (@event.AddMethod != null ? $"_payload.{@event.Name} += value;" : null),
                (@event.RemoveMethod != null ? $"_payload.{@event.Name} -= value;" : null)
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

            return new IndexerProduct(
                "public",
                indexer,
                (indexer.GetMethod != null ? $"return _payload[{parameters}];" : null),
                (indexer.SetMethod != null ? $"_payload[{parameters}] = value;" : null)
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
                (property.GetMethod != null ? $"return _payload.{property.Name};" : null),
                (property.SetMethod != null ? $"_payload.{property.Name} = value;" : null)
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
            var constructorArguments = extractor.GetMethodArguments(method);


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


    }
}
