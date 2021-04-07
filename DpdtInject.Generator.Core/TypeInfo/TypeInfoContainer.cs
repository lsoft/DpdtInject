using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Core.TypeInfo
{
    public abstract class TypeInfoContainer : ITypeInfoContainer
    {
        protected Compilation _compilation;
        private readonly List<ClassDeclarationSyntax>? _candidateClasses;

        private readonly HashSet<ITypeSymbol> _factoryTargetTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        private readonly HashSet<ITypeSymbol> _proxyTargetTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        protected TypeInfoContainer(
            Compilation compilation,
            List<ClassDeclarationSyntax>? candidateClasses = null
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            _compilation = compilation;
            _candidateClasses = candidateClasses;
        }

        public SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
        {
            return _compilation.GetSemanticModel(syntaxTree);
        }

        public INamedTypeSymbol? GetTypeByMetadataName(string fullyQualifiedMetadataName)
        {
            return _compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }


        public IEnumerable<INamedTypeSymbol> GetClusterTypes(Func<INamedTypeSymbol, bool> predicate)
        {
            //Debugger.Launch();

            if (_candidateClasses != null && _candidateClasses.Count > 0)
            {
                var result = new Dictionary<string, INamedTypeSymbol>();
                foreach (var candidateClass in _candidateClasses)
                {
                    SemanticModel model = _compilation.GetSemanticModel(candidateClass.SyntaxTree);
                    if (model == null)
                    {
                        continue;
                    }

                    var symbol = model.GetDeclaredSymbol(candidateClass);
                    if (symbol == null)
                    {
                        continue;
                    }

                    var globalName = symbol.ToGlobalDisplayString();
                    if (result.ContainsKey(globalName))
                    {
                        continue;
                    }

                    if (!predicate(symbol))
                    {
                        continue;
                    }

                    result[globalName] = symbol;
                }

                if (result.Count > 0)
                {
                    return result.Values;
                }

                //nobody from the candidates does match
                //switch back to full scan
            }

            var scanned = _compilation.Assembly.GlobalNamespace.GetAllTypes(predicate);
            return scanned;
        }

        public void AddSources(ModificationDescription[] modificationDescriptions)
        {
            AddSourcesInternal(modificationDescriptions);
        }

        /// <inheritdoc />
        public void AddAdditionalFile(string xmlBody)
        {
            AddAdditionalFileInternal(xmlBody);
        }

        public bool IsFactoryBuildFor(ITypeSymbol factoryTargetType)
        {
            if (factoryTargetType is null)
            {
                throw new ArgumentNullException(nameof(factoryTargetType));
            }

            return _factoryTargetTypes.Contains(factoryTargetType);
        }

        public void AddFactoryBuildFor(ITypeSymbol factoryTargetType)
        {
            _factoryTargetTypes.Add(factoryTargetType);
        }

        public bool IsProxyBuildFor(ITypeSymbol proxyTargetType)
        {
            if (proxyTargetType is null)
            {
                throw new ArgumentNullException(nameof(proxyTargetType));
            }

            return _proxyTargetTypes.Contains(proxyTargetType);
        }

        public void AddProxyBuildFor(ITypeSymbol proxyTargetType)
        {
            _proxyTargetTypes.Add(proxyTargetType);
        }

        protected abstract void AddSourcesInternal(ModificationDescription[] modificationDescriptions);

        protected abstract void AddAdditionalFileInternal(string xmlBody);

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
