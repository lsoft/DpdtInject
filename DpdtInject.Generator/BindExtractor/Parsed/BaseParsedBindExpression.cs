using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DpdtInject.Generator.Binding;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.BindExtractor.Parsed
{
    public abstract class BaseParsedBindExpression : IParsedBindExpression
    {

        /// <inheritdoc />
        public abstract ExpressionStatementSyntax ExpressionNode
        {
            get;
        }

        /// <inheritdoc />
        public abstract BindScopeEnum Scope
        {
            get;
        }


        /// <inheritdoc />
        public abstract void Validate();

        /// <inheritdoc />
        public abstract IBindingContainer CreateBindingContainer();



        protected static ArgumentSyntax? DetermineArgumentSubClause(
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols,
            params MethodInfo[] methods
            )
        {
            if (methods is null)
            {
                throw new ArgumentNullException(nameof(methods));
            }

            foreach (var method in methods)
            {
                var when = invocationSymbols.FirstOrDefault(s => s.Item2.ContainingType.ToDisplayString() == method.DeclaringType!.FullName && s.Item2.Name == method.Name);

                if (when is null)
                {
                    continue;
                }

                var argument0 = when.Item1.ArgumentList.Arguments[0];

                return argument0;
            }

            return null;
        }

    }
}
