using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.From;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.From
{
    public class FromAllInterfaces : IFromTypesProvider
    {
        public ImmutableArray<ITypeSymbol> GetBindFromTypes(ITypeSymbol type)
        {
            var inters = type.AllInterfaces.ToImmutableArray<ITypeSymbol>();
            return inters;
        }
    }
}
