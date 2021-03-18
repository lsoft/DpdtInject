using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DpdtInject.Generator.Core.TypeInfo
{
    public abstract class TypeInfoContainer : ITypeInfoContainer
    {
        protected Compilation _compilation;
        private readonly List<ClassDeclarationSyntax>? _candidateClasses;

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

            //for profiling
            //_candidateClasses = new List<ClassDeclarationSyntax>();
            //foreach (var st in _compilation.SyntaxTrees)
            //{
            //    //var cus = SyntaxFactory.ParseCompilationUnit(File.ReadAllText("C:\\projects\\github\\DpdtInject\\DpdtInject.Tests.Performance\\TimeConsume\\BigTree0\\Singleton\\DpdtCluster.cs"));
            //    _candidateClasses.AddRange(
            //        st.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Where(a => a.Identifier.ToString().Contains("TimeConsumeBigTree0_Cluster")).ToList()
            //        );
            //}
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

                    var fullName = symbol.ToDisplayString();
                    if (result.ContainsKey(fullName))
                    {
                        continue;
                    }

                    if (!predicate(symbol))
                    {
                        continue;
                    }

                    result[fullName] = symbol;
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
