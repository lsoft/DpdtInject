using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DpdtInject.Generator.TypeInfo
{
    public interface ITypeInfoContainer : ITypeInfoProvider
    {
        void AddSources(
            ModificationDescription[] modificationDescriptions
            );
    }

    public interface ITypeInfoProvider
    {
        SemanticModel GetSemanticModel(SyntaxTree syntaxTree);

        INamedTypeSymbol? GetTypeByMetadataName(string fullyQualifiedMetadataName);

        IEnumerable<INamedTypeSymbol> GetAllTypes();
    }
}
