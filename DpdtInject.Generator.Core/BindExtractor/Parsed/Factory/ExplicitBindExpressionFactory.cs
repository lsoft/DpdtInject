using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Core.TypeInfo;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Generator.Core.BindExtractor.Parsed.Factory;
using DpdtInject.Generator.Core.BindExtractor.Parsed;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.From;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory
{
    public class ExplicitBindExpressionFactory : BindExpressionFactory
    {
        private readonly ITypeInfoContainer _typeInfoContainer;
        private readonly SemanticModelDecorator _semanticModel;
        private readonly ConstructorArgumentFromSyntaxExtractor _extractor;
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;

        public ExplicitBindExpressionFactory(
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

        public override IReadOnlyList<IParsedBindExpression> Create(
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

            var result = new List<IParsedBindExpression>();

            var scope = DetermineScope(invocationSymbols);

            switch (scope)
            {
                case BindScopeEnum.Transient:
                case BindScopeEnum.Singleton:
                case BindScopeEnum.Custom:
                    var to = GetTo(invocationSymbols);

                    var fromTypesProvider = FromTypesProviderFactory.CreateFromTypesProvider(invocationSymbols);
                    var from = fromTypesProvider.GetBindFromTypes(to);
                    //var from = invocationSymbols.First(
                    //    s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(DefaultCluster).ToGlobalDisplayString() && s.Item2.Name == DefaultCluster.BindMethodName
                    //    );


                    result.Add(
                        new STCParsedBindExpression(
                            _typeInfoContainer,
                            _extractor,
                            _constructorArgumentDetector,
                            expressionNode,
                            invocationSymbols,
                            from,
                            to,
                            scope,
                            false
                            )
                        );
                    break;

                case BindScopeEnum.Constant:
                    result.Add(
                        new ConstantParsedBindExpression(
                            _semanticModel,
                            expressionNode,
                            invocationSymbols
                            )
                        );
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }
    }
}
