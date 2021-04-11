using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector.Src;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory
{
    public class DetermineBindExpressionFactory : BindExpressionFactory
    {
        private readonly BindExpressionFactory _explicitbef;
        private readonly BindExpressionFactory _conventionalbef;

        public DetermineBindExpressionFactory(
            BindExpressionFactory explicitbef,
            BindExpressionFactory conventionalbef
            )
        {
            if (explicitbef is null)
            {
                throw new ArgumentNullException(nameof(explicitbef));
            }

            if (conventionalbef is null)
            {
                throw new ArgumentNullException(nameof(conventionalbef));
            }

            _explicitbef = explicitbef;
            _conventionalbef = conventionalbef;
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

            if (invocationSymbols[0].Item2.Name == DefaultCluster.ScanInAssembliesWithMethodName && invocationSymbols[0].Item2.ContainingType.ToGlobalDisplayString() == typeof(DefaultCluster).ToGlobalDisplayString())
            {
                return _conventionalbef.Create(expressionNode, invocationSymbols);
            }

            return _explicitbef.Create(expressionNode, invocationSymbols);
        }
    }
}
