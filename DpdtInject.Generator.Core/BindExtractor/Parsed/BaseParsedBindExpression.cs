using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Bind.Settings;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed
{
    public abstract class BaseParsedBindExpression : IParsedBindExpression
    {
        protected readonly List<ISetting> _settings = new List<ISetting>();

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

        public BaseParsedBindExpression(
            BindScopeEnum scope,
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols
            )
        {
            if (invocationSymbols is null)
            {
                throw new ArgumentNullException(nameof(invocationSymbols));
            }

            var settingScopes = new HashSet<string>();

            var setupInvocations = invocationSymbols.FindAll(p => p.Item2.Name == nameof(IConfigureAndConditionalBinding.Setup));
            foreach (var setupInvocation in setupInvocations)
            {
                var settingSymbol = setupInvocation.Item2.TypeArguments.First();

                if (settingSymbol.BaseType is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectSetting,
                        $"Incorrect setting used [{settingSymbol.ToDisplayString()}]",
                        settingSymbol.ToDisplayString()
                        );
                }

                if (settingScopes.Contains(settingSymbol.BaseType.ToDisplayString()))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectSetting,
                        $"Only one settings of each scope is allowed [{settingSymbol.BaseType.ToDisplayString()}]",
                        settingSymbol.BaseType.ToDisplayString()
                        );
                }
                settingScopes.Add(settingSymbol.BaseType.ToDisplayString());

                var settingType = typeof(ISetting).Assembly.GetType(
                    settingSymbol.ToDisplayString()
                    );

                if (settingType is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Internal error with instanciate a [{settingSymbol.ToDisplayString()}]",
                        settingSymbol.BaseType.ToDisplayString()
                        );
                }

                var settingInstance = (ISetting)Activator.CreateInstance(settingType)!;

                if (!settingInstance.IsAllowedFor(scope))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectSetting,
                        $"Setting {settingInstance.GetType().Name} is incompatible with scope {scope}"
                        );
                }

                _settings.Add(settingInstance);
            }
        }

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
