using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer.ClassProducer.Product;
using DpdtInject.Generator.Core.Producer.ClassProducer.Proxy;
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

        private readonly IProxyMemberFactory _notProxiedMemberFactory = new NotProxiedMemberFactory();
        private readonly IProxyMemberFactory _proxiedMemberFactory = new ProxiedMemberFactory();

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
                    (m.Kind.In(SymbolKind.Property, SymbolKind.Event))
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

                var factory =
                    telemetryAttribute is not null
                        ? _proxiedMemberFactory
                        : _notProxiedMemberFactory
                        ;


                if (member is IPropertySymbol property && !property.IsIndexer)
                {
                    var declaredPropertyProduct = factory.GetPropertyProduct(
                        property
                        );

                    result.Add(declaredPropertyProduct);
                }
                if (member is IPropertySymbol indexer && indexer.IsIndexer)
                {
                    var declaredPropertyProduct = factory.GetIndexerProduct(
                        indexer
                        );

                    result.Add(declaredPropertyProduct);
                }
                if (member is IEventSymbol @event)
                {
                    var declaredMethodProduct = factory.GetEventProduct(
                        @event
                        );

                    result.Add(declaredMethodProduct);
                }
                if (member is IMethodSymbol method)
                {
                    var declaredMethodProduct = factory.GetMethodProduct(
                        method
                        );

                    result.Add(declaredMethodProduct);
                }
            }

            return result;
        }

    }
}
