using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.Conventional.From
{
    public static class FromTypesProviderFactory
    {
        public static IFromTypesProvider CreateFromTypesProvider(
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols
            )
        {
            if (invocationSymbols is null)
            {
                throw new ArgumentNullException(nameof(invocationSymbols));
            }

            var fai = invocationSymbols
                .FirstOrDefault(
                    s => s.Item2.ContainingType.ToDisplayString() == typeof(IConventionalBinding2).FullName && s.Item2.Name == nameof(IConventionalBinding2.FromAllInterfaces)
                    );
            if (fai != null)
            {
                return new FromAllInterfaces();
            }

            var fitself = invocationSymbols
                .FirstOrDefault(
                    s => s.Item2.ContainingType.ToDisplayString() == typeof(IConventionalBinding2).FullName && s.Item2.Name == nameof(IConventionalBinding2.FromItself)
                    );
            if (fitself != null)
            {
                return new FromItself();
            }

            var fst = invocationSymbols
                .FirstOrDefault(
                    s => s.Item2.ContainingType.ToDisplayString() == typeof(IConventionalBinding2).FullName && s.Item2.Name == nameof(IConventionalBinding2.From)
                    );
            if (fst != null)
            {
                return new FromSpecificTypes(fst.Item2.TypeArguments);
            }

            throw new DpdtException(
                DpdtExceptionTypeEnum.IncorrectBinding_IncorrectClause,
                $"Cannot determine Bind-From clause"
                );
        }
    }
}
