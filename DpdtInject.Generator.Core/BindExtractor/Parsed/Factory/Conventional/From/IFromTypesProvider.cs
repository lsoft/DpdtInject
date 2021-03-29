using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.Conventional.From
{
    public interface IFromTypesProvider
    {
        ImmutableArray<ITypeSymbol> GetBindFromTypes(INamedTypeSymbol type);
    }
}
