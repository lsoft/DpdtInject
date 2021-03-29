using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.Conventional.From
{
    public class FromItself : IFromTypesProvider
    {
        public ImmutableArray<ITypeSymbol> GetBindFromTypes(INamedTypeSymbol type)
        {
            var inters = new[] { type }.ToImmutableArray<ITypeSymbol>();
            return inters;
        }
    }
}
