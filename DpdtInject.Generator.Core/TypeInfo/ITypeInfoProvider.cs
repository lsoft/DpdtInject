using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Core.TypeInfo
{
    public interface ITypeInfoProvider
    {
        SemanticModel GetSemanticModel(SyntaxTree syntaxTree);

        INamedTypeSymbol? GetTypeByMetadataName(string fullyQualifiedMetadataName);

        IEnumerable<INamedTypeSymbol> GetClusterTypes(Func<INamedTypeSymbol, bool> predicate);
    }
}
