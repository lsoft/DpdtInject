using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using DpdtInject.Injector.Bind;

namespace DpdtInject.Generator.Core.BindExtractor
{

    public class ConstructorArgumentFromSyntaxExtractor
        : CSharpSyntaxRewriter
    {
        private readonly List<DetectedMethodArgument> _constructorArguments;
        private readonly SemanticModelDecorator _semanticModel;

        public ConstructorArgumentFromSyntaxExtractor(
            SemanticModelDecorator semanticModel
            )
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            _semanticModel = semanticModel;

            _constructorArguments = new List<DetectedMethodArgument>();
        }

        public void ClearAndVisit(SyntaxNode? syntaxNode)
        {
            _constructorArguments.Clear();
            Visit(syntaxNode);
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (node.Type.ToString() != nameof(ConstructorArgument))
            {
                return base.VisitObjectCreationExpression(node)!;
            }
            if (node.ArgumentList == null || node.ArgumentList.Arguments == null || node.ArgumentList.Arguments.Count == 0)
            {
                return base.VisitObjectCreationExpression(node)!;
            }

            var constructorArgumentNameNode = node.ArgumentList.Arguments[0];
            if (!constructorArgumentNameNode.Expression.TryGetCompileTimeString(_semanticModel, out var argument))
            {
                throw new Exception(@"Constructor argument name should be direct-defined string or const string. Dpdt syntax parser does not support other options.");
            }

            var constructorArgumentBodyNode = node.ArgumentList.Arguments[1];
            var body = constructorArgumentBodyNode.ToString();

            _constructorArguments.Add(
                new DetectedMethodArgument(
                    argument,
                    body
                    )
                );

            return base.VisitObjectCreationExpression(node)!;
        }

        internal List<DetectedMethodArgument> GetConstructorArguments()
        {
            return new(_constructorArguments);
        }
    }
}
