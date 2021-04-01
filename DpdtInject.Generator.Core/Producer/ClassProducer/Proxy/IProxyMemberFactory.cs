using DpdtInject.Generator.Core.Producer.Product;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Producer.ClassProducer.Proxy
{
    public interface IProxyMemberFactory
    {
        EventProduct GetEventProduct(
            IEventSymbol @event
            );

        IndexerProduct GetIndexerProduct(
            IPropertySymbol indexer
            );

        PropertyProduct GetPropertyProduct(
            IPropertySymbol property
            );

        IMethodProduct GetMethodProduct(
            IMethodSymbol method
            );
    }
}
