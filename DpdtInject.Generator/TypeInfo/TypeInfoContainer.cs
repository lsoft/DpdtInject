using DpdtInject.Generator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.TypeInfo
{
    public abstract class TypeInfoContainer : ITypeInfoContainer
    {
        protected Compilation _compilation;

        public TypeInfoContainer(
            Compilation compilation
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            _compilation = compilation;
        }

        public SemanticModel GetSemanticModel(SyntaxTree syntaxTree) => _compilation.GetSemanticModel(syntaxTree);

        public INamedTypeSymbol? GetTypeByMetadataName(string fullyQualifiedMetadataName)
        {
            return _compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }

        public IEnumerable<INamedTypeSymbol> GetAllTypes()
        {
            return _compilation.GlobalNamespace.GetAllTypes();
        }

        public abstract void AddSource(ModificationDescription modificationDescription);

        protected void UpdateCompilationWith(SourceText sourceText)
        {
            if (sourceText is null)
            {
                throw new ArgumentNullException(nameof(sourceText));
            }

            var options = ((CSharpCompilation)_compilation).SyntaxTrees[0].Options;
            var sourceTree = CSharpSyntaxTree.ParseText(sourceText, (CSharpParseOptions)options);
            _compilation = _compilation.AddSyntaxTrees(sourceTree);
        }
    }
}
