using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.From
{
    public class FromItself : IFromTypesProvider
    {
        public ImmutableArray<ITypeSymbol> GetBindFromTypes(ITypeSymbol type)
        {
            var inters = new[] { type }.ToImmutableArray<ITypeSymbol>();
            return inters;
        }
    }
}
