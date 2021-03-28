using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory
{
    public abstract class BindExpressionFactory
    {
        public abstract IReadOnlyList<IParsedBindExpression> Create(
            ExpressionStatementSyntax expressionNode,
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols
            );

        protected BindScopeEnum DetermineScope(
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
