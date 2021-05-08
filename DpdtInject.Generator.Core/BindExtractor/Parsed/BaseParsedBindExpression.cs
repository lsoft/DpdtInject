using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Producer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Injector.Src.Bind.Settings;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Excp;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed
{
    public abstract class BaseParsedBindExpression : IParsedBindExpression
    {
        protected readonly List<ISetting> _settings = new List<ISetting>();
        protected readonly BindScopeEnum _scope;


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

            _scope = scope;

            var settingScopes = new HashSet<string>();

            var setupInvocations = invocationSymbols.FindAll(p => p.Item2.Name == nameof(IConfigureAndConditionalBinding.Setup));
            foreach (var setupInvocation in setupInvocations)
            {
                var settingSymbol = setupInvocation.Item2.TypeArguments.First();

                if (settingSymbol.BaseType is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectSetting,
                        $"Incorrect setting used [{settingSymbol.ToGlobalDisplayString()}]",
                        settingSymbol.ToGlobalDisplayString()
                        );
                }

                var reflectionTypeString = ((INamedTypeSymbol)settingSymbol).ToReflectionFormat();

                var settingType = typeof(ISetting).Assembly.GetType(
                    reflectionTypeString
                    );

                if (settingType is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Internal error with instanciate a [{settingSymbol.ToGlobalDisplayString()}]",
                        settingSymbol.BaseType.ToGlobalDisplayString()
                        );
                }
                
                var settingInstance = (ISetting)Activator.CreateInstance(settingType)!;

                if (settingScopes.Contains(settingInstance.Scope))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectSetting,
                        $"Only one settings of each scope is allowed [{settingInstance.Scope}]",
                        settingSymbol.BaseType.ToGlobalDisplayString()
                        );
                }
                settingScopes.Add(settingInstance.Scope);


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
                var when = invocationSymbols.FirstOrDefault(s => s.Item2.ContainingType.ToGlobalDisplayString() == method.DeclaringType!.ToGlobalDisplayString() && s.Item2.Name == method.Name);

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
