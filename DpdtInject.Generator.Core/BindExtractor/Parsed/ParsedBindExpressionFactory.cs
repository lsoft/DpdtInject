using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Core.TypeInfo;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed
{
    public class ParsedBindExpressionFactory
    {
        private readonly ITypeInfoContainer _typeInfoContainer;
        private readonly SemanticModelDecorator _semanticModel;
        private readonly ConstructorArgumentFromSyntaxExtractor _extractor;
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;

        public ParsedBindExpressionFactory(
            ITypeInfoContainer typeInfoContainer,
            SemanticModelDecorator semanticModel,
            ConstructorArgumentFromSyntaxExtractor extractor,
            ConstructorArgumentDetector constructorArgumentDetector
            )
        {
            if (typeInfoContainer is null)
            {
                throw new ArgumentNullException(nameof(typeInfoContainer));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (extractor is null)
            {
                throw new ArgumentNullException(nameof(extractor));
            }

            if (constructorArgumentDetector is null)
            {
                throw new ArgumentNullException(nameof(constructorArgumentDetector));
            }

            _typeInfoContainer = typeInfoContainer;
            _semanticModel = semanticModel;
            _extractor = extractor;
            _constructorArgumentDetector = constructorArgumentDetector;
        }

        public IParsedBindExpression Create(
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

            var scope = DetermineScope(invocationSymbols);

            switch (scope)
            {
                case BindScopeEnum.Transient:
                case BindScopeEnum.Singleton:
                case BindScopeEnum.Custom:
                    return new STCParsedBindExpression(
                        _typeInfoContainer,
                        _extractor,
                        _constructorArgumentDetector,
                        expressionNode,
                        invocationSymbols,
                        scope
                        );

                case BindScopeEnum.Constant:
                    return new ConstantParsedBindExpression(
                        _semanticModel,
                        expressionNode,
                        invocationSymbols
                        );


                default:
                    throw new ArgumentOutOfRangeException();
            }

        }


        private BindScopeEnum DetermineScope(
            IReadOnlyList<Tuple<InvocationExpressionSyntax, IMethodSymbol>> symbols
            )
        {
            if (symbols.Any(s => s.Item2.ContainingType.ToDisplayString() == typeof(IScopeBinding).FullName && s.Item2.Name == nameof(IScopeBinding.WithSingletonScope)))
            {
                return BindScopeEnum.Singleton;
            }
            if (symbols.Any(s => s.Item2.ContainingType.ToDisplayString() == typeof(IScopeBinding).FullName && s.Item2.Name == nameof(IScopeBinding.WithTransientScope)))
            {
                return BindScopeEnum.Transient;
            }
            if (symbols.Any(s => s.Item2.ContainingType.ToDisplayString() == typeof(IToOrConstantBinding).FullName && s.Item2.Name == nameof(IToOrConstantBinding.WithConstScope)))
            {
                return BindScopeEnum.Constant;
            }
            if (symbols.Any(s => s.Item2.ContainingType.ToDisplayString() == typeof(IScopeBinding).FullName && s.Item2.Name == nameof(IScopeBinding.WithCustomScope)))
            {
                return BindScopeEnum.Custom;
            }

            throw new InvalidOperationException("unknown scope");
        }

    }
}