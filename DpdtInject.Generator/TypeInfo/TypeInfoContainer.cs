using DpdtInject.Generator.Helpers;
using DpdtInject.Injector.Helper;
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

        protected TypeInfoContainer(
            Compilation compilation
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            _compilation = compilation;
        }

        public SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
        {
            return _compilation.GetSemanticModel(syntaxTree);
        }

        public INamedTypeSymbol? GetTypeByMetadataName(string fullyQualifiedMetadataName)
        {
            //_compilation.meta
            return _compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }

        public IEnumerable<INamedTypeSymbol> GetAllTypes()
        {
            return _compilation.GlobalNamespace.GetAllTypes();
        }

        public abstract void AddSources(ModificationDescription[] modificationDescriptions);

        protected void UpdateCompilationWith(SourceText[] sourceTexts)
        {
            if (sourceTexts is null)
            {
                throw new ArgumentNullException(nameof(sourceTexts));
            }

            var options = ((CSharpCompilation)_compilation).SyntaxTrees[0].Options;
            var sourceTrees = sourceTexts.ConvertAll(sourceText => CSharpSyntaxTree.ParseText(sourceText, (CSharpParseOptions)options));
            _compilation = _compilation.AddSyntaxTrees(sourceTrees);
        }
    }
}
