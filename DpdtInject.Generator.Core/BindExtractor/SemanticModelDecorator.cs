using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core.BindExtractor
{
    /// <summary>
    /// Semantic model decorator that encapsulate a few semantic models.
    /// </summary>
    public class SemanticModelDecorator
    {
        private readonly IReadOnlyList<SemanticModel> _semanticModels;

        public SemanticModelDecorator(
            IReadOnlyList<SemanticModel> semanticModels
            )
        {
            if (semanticModels is null)
            {
                throw new ArgumentNullException(nameof(semanticModels));
            }

            _semanticModels = semanticModels;
        }

        public Microsoft.CodeAnalysis.TypeInfo GetTypeInfo(
            ExpressionSyntax typeSyntax
            )
        {
            if (typeSyntax is null)
            {
                throw new ArgumentNullException(nameof(typeSyntax));
            }

            return Perform(
                semanticModel => semanticModel.GetTypeInfo(typeSyntax)
                );
        }

        public Microsoft.CodeAnalysis.SymbolInfo GetSymbolInfo(
            SyntaxNode syntaxNode
            )
        {
            if (syntaxNode is null)
            {
                throw new ArgumentNullException(nameof(syntaxNode));
            }

            return Perform(
                semanticModel => semanticModel.GetSymbolInfo(syntaxNode)
                );
        }

        public Optional<object?> GetConstantValue(
            SyntaxNode syntaxNode
            )
        {
            if (syntaxNode is null)
            {
                throw new ArgumentNullException(nameof(syntaxNode));
            }

            return Perform(
                semanticModel => semanticModel.GetConstantValue(syntaxNode)
                );
        }

        private TOut Perform<TOut>(
            Func<SemanticModel, TOut> performFunc
            )
        {
            if (performFunc is null)
            {
                throw new ArgumentNullException(nameof(performFunc));
            }

            for (var i = 0; i < _semanticModels.Count; i++)
            {
                var semanticModel = _semanticModels[i];

                try
                {
                    var result = performFunc(semanticModel);

                    return result;
                }
                catch
                {
                    if (i == _semanticModels.Count - 1)
                    {
                        //last semantic model failed to perform its job
                        throw;
                    }

                    //we have more semantic models, continue
                }
            }

            throw new InvalidOperationException("This line should never be executed");
        }

    }
}
