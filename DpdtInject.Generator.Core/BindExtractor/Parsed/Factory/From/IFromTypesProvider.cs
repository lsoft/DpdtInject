using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.From
{
    public interface IFromTypesProvider
    {
        ImmutableArray<ITypeSymbol> GetBindFromTypes(ITypeSymbol type);
    }
}
