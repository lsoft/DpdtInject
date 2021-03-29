using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.Conventional.From
{
    public class FromAllInterfaces : IFromTypesProvider
    {
        public ImmutableArray<ITypeSymbol> GetBindFromTypes(INamedTypeSymbol type)
        {
            var inters = type.AllInterfaces.ToImmutableArray<ITypeSymbol>();
            return inters;
        }
    }
}
