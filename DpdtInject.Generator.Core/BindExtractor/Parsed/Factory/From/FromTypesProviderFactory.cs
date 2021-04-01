using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.From
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

            var fdc = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(DefaultCluster).ToGlobalDisplayString() && s.Item2.Name == DefaultCluster.BindMethodName
                );
            if (fdc != null)
            {
                return new FromSpecificTypes(fdc.Item2.TypeArguments, true);
            }

            var fai0 = invocationSymbols
                .FirstOrDefault(
                    s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(DefaultCluster).ToGlobalDisplayString() && s.Item2.Name == DefaultCluster.BindAllInterfacesMethodName
                    );
            if (fai0 != null)
            {
                return new FromAllInterfaces();
            }

            var fai1 = invocationSymbols
                .FirstOrDefault(
                    s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IConventionalBinding2).ToGlobalDisplayString() && s.Item2.Name == nameof(IConventionalBinding2.FromAllInterfaces)
                    );
            if (fai1 != null)
            {
                return new FromAllInterfaces();
            }


            var fitself = invocationSymbols
                .FirstOrDefault(
                    s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IConventionalBinding2).ToGlobalDisplayString() && s.Item2.Name == nameof(IConventionalBinding2.FromItself)
                    );
            if (fitself != null)
            {
                return new FromItself();
            }

            var fst = invocationSymbols
                .FirstOrDefault(
                    s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IConventionalBinding2).ToGlobalDisplayString() && s.Item2.Name == nameof(IConventionalBinding2.From)
                    );
            if (fst != null)
            {
                return new FromSpecificTypes(fst.Item2.TypeArguments, false);
            }

            throw new DpdtException(
                DpdtExceptionTypeEnum.IncorrectBinding_IncorrectClause,
                $"Cannot determine Bind-From clause"
                );
        }
    }
}
