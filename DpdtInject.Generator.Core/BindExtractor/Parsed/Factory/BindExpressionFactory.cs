using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Excp;
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



        public ITypeSymbol GetTo(
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols,
            ITypeSymbol? processedType = null
            )
        {
            var pair = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IToOrConstantBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IToOrConstantBinding.To)
                );

            if (pair is not null)
            {
                return pair.Item2.TypeArguments.First();
            }

            pair = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IToOrConstantBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IToOrConstantBinding.ToIsolatedFactory)
                );

            if (pair is not null)
            {
                return pair.Item2.TypeArguments.First();
            }

            pair = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IToOrConstantBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IToOrConstantBinding.ToProxy)
                );

            if (pair is not null)
            {
                return pair.Item2.TypeArguments.First();
            }

            pair = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IConventionalBinding3).ToGlobalDisplayString() && s.Item2.Name == nameof(IConventionalBinding3.ToItself)
                );

            if (pair is not null)
            {
                if (processedType is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Unknown problem to set processed type"
                        );
                }

                return processedType;
            }

            throw new DpdtException(
                DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                $"Unknown problem to access 'To' clause"
                );
        }


        protected BindScopeEnum DetermineScope(
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> symbols
            )
        {
            if (symbols.Any(s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IScopeBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IScopeBinding.WithSingletonScope)))
            {
                return BindScopeEnum.Singleton;
            }
            if (symbols.Any(s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IScopeBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IScopeBinding.WithTransientScope)))
            {
                return BindScopeEnum.Transient;
            }
            if (symbols.Any(s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IToOrConstantBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IToOrConstantBinding.WithConstScope)))
            {
                return BindScopeEnum.Constant;
            }
            if (symbols.Any(s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IScopeBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IScopeBinding.WithCustomScope)))
            {
                return BindScopeEnum.Custom;
            }

            throw new DpdtException(
                DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                $"Unknown scope"
                );
        }
    }
}
