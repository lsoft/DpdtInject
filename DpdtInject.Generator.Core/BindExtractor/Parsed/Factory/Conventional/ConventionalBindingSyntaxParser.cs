using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.From;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Helper;
using DpdtInject.Injector.Src;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.Conventional
{
    public class ConventionalBindingSyntaxParser

    {
        public ExpressionStatementSyntax ExpressionNode
        {
            get;
        }
        public IReadOnlyList<Tuple<InvocationExpressionSyntax, IMethodSymbol>> InvocationSymbols
        {
            get;
        }
        public IReadOnlyList<ITypeSymbol> ScanInList
        {
            get;
        }
        public IReadOnlyList<ITypeSymbol> SelectWithSet
        {
            get;
        }
        public IReadOnlyList<ITypeSymbol> ExcludeWithSet
        {
            get;
        }
        public IFromTypesProvider FromTypesProvider
        {
            get;
        }


        public ConventionalBindingSyntaxParser(
            ExpressionStatementSyntax expressionNode,
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols
            )
        {
            if (expressionNode is null)
            {
                throw new ArgumentNullException(nameof(expressionNode));
            }

            if (invocationSymbols is null)
            {
                throw new ArgumentNullException(nameof(invocationSymbols));
            }

            ExpressionNode = expressionNode;
            InvocationSymbols = invocationSymbols;

            ScanInList = invocationSymbols
                .Where(
                    s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(DefaultCluster).ToGlobalDisplayString() && s.Item2.Name == DefaultCluster.ScanInAssembliesWithMethodName
                    )
                .SelectMany(s => s.Item2.TypeArguments)
                .ToList()
                ;

            var selectWithSet = new List<ITypeSymbol>();
            selectWithSet.AddRange(
                invocationSymbols
                    .Where(
                        s => s.Item2.ContainingType.ToGlobalDisplayString().In(typeof(IConventionalBinding).ToGlobalDisplayString(), typeof(IConventionalBinding2).ToGlobalDisplayString()) && s.Item2.Name == nameof(IConventionalBinding.SelectAllWith)
                        )
                    .SelectMany(s => s.Item2.TypeArguments)
                );
            selectWithSet.AddRange(
                invocationSymbols
                    .Where(
                        s => s.Item2.ContainingType.ToGlobalDisplayString().In(typeof(IConventionalBinding).ToGlobalDisplayString(), typeof(IConventionalBinding2).ToGlobalDisplayString()) && s.Item2.Name == nameof(IConventionalBinding.SelectAllWithOpenGeneric)
                        )
                    .SelectMany(s => s.Item2.TypeArguments)
                    .Select(ta => ta as INamedTypeSymbol)
                    .Where(nta => nta != null)
                    .Select(nta => nta!.OriginalDefinition)
                );
            SelectWithSet = selectWithSet;

            var excludeWithSet = new List<ITypeSymbol>();
            excludeWithSet.AddRange(
                invocationSymbols
                    .Where(
                        s => s.Item2.ContainingType.ToGlobalDisplayString().In(typeof(IConventionalBinding).ToGlobalDisplayString(), typeof(IConventionalBinding2).ToGlobalDisplayString()) && s.Item2.Name == nameof(IConventionalBinding2.ExcludeAllWith)
                        )
                    .SelectMany(s => s.Item2.TypeArguments)
                );
            excludeWithSet.AddRange(
                invocationSymbols
                    .Where(
                        s => s.Item2.ContainingType.ToGlobalDisplayString().In(typeof(IConventionalBinding).ToGlobalDisplayString(), typeof(IConventionalBinding2).ToGlobalDisplayString()) && s.Item2.Name == nameof(IConventionalBinding2.ExcludeAllWithOpenGeneric)
                        )
                    .SelectMany(s => s.Item2.TypeArguments)
                    .Select(ta => ta as INamedTypeSymbol)
                    .Where(nta => nta != null)
                    .Select(nta => nta!.OriginalDefinition)
                );
            ExcludeWithSet = excludeWithSet;

            FromTypesProvider = FromTypesProviderFactory.CreateFromTypesProvider(invocationSymbols);
        }

        public IReadOnlyCollection<IAssemblySymbol> GetAssemblesOfInterest()
        {
            var assemblies = new HashSet<IAssemblySymbol>(SymbolEqualityComparer.Default);
            foreach (var type in ScanInList)
            {
                assemblies.Add(type.ContainingAssembly);
            }

            return assemblies;
        }
    }
}
