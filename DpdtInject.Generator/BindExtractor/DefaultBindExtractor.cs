using DpdtInject.Generator.Binding;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.BindExtractor.Parsed;
using DpdtInject.Injector;
using DpdtInject.Injector.Bind;

namespace DpdtInject.Generator.BindExtractor
{
    public class DefaultBindExtractor : CSharpSyntaxRewriter
    {
        private readonly SemanticModelDecorator _semanticModel;
        private readonly ParsedBindExpressionFactory _pbeFactory;

        private readonly List<IBindingContainer> _bindingContainers;

        public IReadOnlyList<IBindingContainer> BindingContainers => _bindingContainers;

        public DefaultBindExtractor(
            SemanticModelDecorator semanticModel,
            ParsedBindExpressionFactory pbeFactory
            )
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (pbeFactory is null)
            {
                throw new ArgumentNullException(nameof(pbeFactory));
            }

            _semanticModel = semanticModel;
            _pbeFactory = pbeFactory;

            _bindingContainers = new List<IBindingContainer>();
        }


        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax expressionNode)
        {
            if (expressionNode is null)
            {
                throw new ArgumentNullException(nameof(expressionNode));
            }

            #region it is what we want?

            var invocations = expressionNode
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Reverse()
                .ToList()
                ;

            if (invocations.Count < 2)
            {
                return base.VisitExpressionStatement(expressionNode)!;
            }

            var invocationSymbols = new List<Tuple<InvocationExpressionSyntax, IMethodSymbol>>();
            foreach (var invocation in invocations)
            {
                var symbol = _semanticModel.GetSymbolInfo(invocation).Symbol;

                if (symbol is null)
                {
                    continue;
                }
                if (symbol is not IMethodSymbol)
                {
                    continue;
                }
                if (symbol.ContainingType.ToDisplayString().NotIn(
                        typeof(DefaultCluster).FullName,
                        typeof(IToOrConstantBinding).FullName,
                        typeof(IToFactoryBinding).FullName,
                        typeof(IToProxyBinding).FullName,
                        typeof(IScopeBinding).FullName,
                        typeof(IConfigureAndConditionalBinding).FullName,
                        typeof(IConditionalBinding).FullName,
                        typeof(IConfigureBinding).FullName,
                        typeof(IConstantConditionalBinding).FullName
                        )
                    )
                {
                    continue;
                }

                invocationSymbols.Add(
                    new Tuple<InvocationExpressionSyntax, IMethodSymbol>(
                        invocation,
                        (IMethodSymbol)symbol
                        )
                    );
            }

            var firstInvocationSymbol = invocationSymbols.First();

            if (firstInvocationSymbol.Item2.ContainingType.ToDisplayString().NotIn(
                    typeof(DefaultCluster).FullName
                    )
                )
            {
                return base.VisitExpressionStatement(expressionNode)!;
            }

            var secondInvocationSymbol = invocationSymbols.Second();

            if (secondInvocationSymbol.Item2.ContainingType.ToDisplayString().NotIn(
                    typeof(IToOrConstantBinding).FullName
                    )
                )
            {
                return base.VisitExpressionStatement(expressionNode)!;
            }

            var thirdInvocationSymbol = invocationSymbols.NThOrDefault(2);
            if (thirdInvocationSymbol is not null)
            {
                if (thirdInvocationSymbol.Item2.ContainingType.ToDisplayString().NotIn(
                        typeof(IScopeBinding).FullName,
                        typeof(IToFactoryBinding).FullName,
                        typeof(IToProxyBinding).FullName,
                        typeof(IConstantConditionalBinding).FullName
                        )
                    )
                {
                    return base.VisitExpressionStatement(expressionNode)!;
                }
            }


            var fourthInvocationSymbol = invocationSymbols.NThOrDefault(3);
            if (fourthInvocationSymbol is not null)
            {
                if (fourthInvocationSymbol.Item2.ContainingType.ToDisplayString().NotIn(
                        typeof(IConfigureAndConditionalBinding).FullName,
                        typeof(IScopeBinding).FullName,
                        typeof(IConfigureBinding).FullName,
                        typeof(IConditionalBinding).FullName
                        )
                    )
                {
                    return base.VisitExpressionStatement(expressionNode)!;
                }
            }

            for (var i = 4; i < invocationSymbols.Count; i++)
            {
                if (invocationSymbols[i].Item2.ContainingType.ToDisplayString().NotIn(
                        typeof(IConfigureBinding).FullName,
                        typeof(IConditionalBinding).FullName
                        )
                    )
                {
                    return base.VisitExpressionStatement(expressionNode)!;
                }
            }

            var pbe = _pbeFactory.Create(
                expressionNode,
                invocationSymbols
                );

            pbe.Validate();

            #endregion

            //looks like we found what we want

            var bindingContainer = pbe.CreateBindingContainer(
                );

            _bindingContainers.Add(bindingContainer);

            return expressionNode;
        }
    }
}
