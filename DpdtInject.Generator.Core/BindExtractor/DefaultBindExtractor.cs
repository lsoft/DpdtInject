using DpdtInject.Generator.Core.Binding;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Core.BindExtractor.Parsed;
using DpdtInject.Injector;
using DpdtInject.Injector.Bind;
using System.Reflection;
using DpdtInject.Injector.Excp;
using DpdtInject.Generator.Core.Helpers;

namespace DpdtInject.Generator.Core.BindExtractor
{
    public class DefaultBindExtractor : CSharpSyntaxRewriter
    {
        private static readonly IReadOnlyDictionary<string, Type> _set;

        private readonly SemanticModelDecorator _semanticModel;
        private readonly ParsedBindExpressionFactory _pbeFactory;

        private readonly List<IBindingContainer> _bindingContainers;

        public IReadOnlyList<IBindingContainer> BindingContainers => _bindingContainers;

        static DefaultBindExtractor()
        {
            _set =
                new[]
                {
                    typeof(DefaultCluster),
                    typeof(IToOrConstantBinding),
                    typeof(IToFactoryBinding),
                    typeof(IToProxyBinding),
                    typeof(IScopeBinding),
                    typeof(IConfigureAndConditionalBinding),
                    typeof(IConditionalBinding),
                    typeof(IConfigureBinding),
                    typeof(IConstantConditionalBinding)
                }.ToDictionary(s => s.FullName!, s => s);
        }

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

            var invocations = new List<InvocationExpressionSyntax>();

            var child = expressionNode
                .ChildNodes()
                .OfType<InvocationExpressionSyntax>()
                .FirstOrDefault()
                ;
            while (child is not null)
            {
                invocations.Add(child);

                var preChild = child
                    .ChildNodes()
                    .OfType<MemberAccessExpressionSyntax>()
                    .FirstOrDefault()
                    ;

                if (preChild is null)
                {
                    break;
                }

                child = preChild
                    .ChildNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .FirstOrDefault() 
                    ;
            }

            if (invocations.Count < 2)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectClause,
                    $"Incorrect bind expression"
                    );
            }

            invocations.Reverse();

            var currentType = _set[typeof(DefaultCluster).FullName!];

            var invocationSymbols = new List<Tuple<InvocationExpressionSyntax, IMethodSymbol>>();
            for(var invocationIndex = 0; invocationIndex < invocations.Count; invocationIndex++)
            {
                var invocation = invocations[invocationIndex];
                var isLast = invocationIndex == (invocations.Count - 1);

                var symbol = _semanticModel.GetSymbolInfo(invocation).Symbol;

                if (symbol is null)
                {
                    continue;
                }
                if (symbol is not IMethodSymbol)
                {
                    continue;
                }

                var setds = symbol.ContainingType.ToDisplayString();
                if (!_set.ContainsKey(setds))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectClause,
                        $"Incorrect clause found in bind expression: {setds}"
                        );
                }

                var methods = currentType.GetAllNonStaticMethodsHierarchy();
                var foundMethod = methods.FirstOrDefault(m => m.Name == symbol.Name);
                if (foundMethod is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectClause,
                        $"Incorrect method found in bind expression: {symbol.Name}"
                        );
                }

                if (isLast && invocations.Count == 2)
                {
                    if (symbol.ContainingType.ToDisplayString() == typeof(IToFactoryBinding).FullName)
                    {
                        throw new DpdtException(
                            DpdtExceptionTypeEnum.IncorrectBinding_IncorrectClause,
                            $"Scope must be defined: {expressionNode}"
                            );
                    }
                }

                currentType = foundMethod.ReturnType;

                invocationSymbols.Add(
                    new Tuple<InvocationExpressionSyntax, IMethodSymbol>(
                        invocation,
                        (IMethodSymbol)symbol
                        )
                    );
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
