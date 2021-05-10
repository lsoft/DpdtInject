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
using DpdtInject.Generator.Core.Binding.Settings.Constructor;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Injector.Src.Bind.Settings.Circular;
using DpdtInject.Generator.Core.Binding.Settings.Circular;
using DpdtInject.Injector.Src.Bind.Settings.CrossCluster;
using DpdtInject.Generator.Core.Binding.Settings.CrossCluster;
using DpdtInject.Generator.Core.Binding.Settings.Wrapper;
using DpdtInject.Injector.Src.Bind.Settings.Wrapper;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed
{
    public abstract class BaseParsedBindExpression : IParsedBindExpression
    {
        protected readonly List<IDefinedSetting> _settings = new List<IDefinedSetting>();
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
                var settingSymbol = (INamedTypeSymbol)setupInvocation.Item2.TypeArguments.First();

                IDefinedSetting setting;
                if (settingSymbol.BaseType != null && settingSymbol.BaseType.ToDisplayString() == typeof(AllAndOrderConstructorSetting).FullName)
                {
                    var constructorSetting = new ConstructorSetting(ConstructorSettingsEnum.AllAndOrder);
                    constructorSetting.AddRange(
                        settingSymbol.TypeArguments
                        );
                    setting = constructorSetting;
                }
                else if (settingSymbol.BaseType != null && settingSymbol.BaseType.ToDisplayString() == typeof(SubsetAndOrderConstructorSetting).FullName)
                {
                    var constructorSetting = new ConstructorSetting(ConstructorSettingsEnum.SubsetAndOrder);
                    constructorSetting.AddRange(
                        settingSymbol.TypeArguments
                        );
                    setting = constructorSetting;
                }
                else if (settingSymbol.BaseType != null && settingSymbol.BaseType.ToDisplayString() == typeof(SubsetNoOrderConstructorSetting).FullName)
                {
                    var constructorSetting = new ConstructorSetting(ConstructorSettingsEnum.SubsetNoOrder);
                    constructorSetting.AddRange(
                        settingSymbol.TypeArguments
                        );
                    setting = constructorSetting;
                }
                else if (settingSymbol.ToDisplayString() == typeof(PerformCircularCheck).FullName)
                {
                    setting = new CircularSetting(true);
                }
                else if (settingSymbol.ToDisplayString() == typeof(SuppressCircularCheck).FullName)
                {
                    setting = new CircularSetting(false);
                }
                else if (settingSymbol.ToDisplayString() == typeof(AllowedCrossCluster).FullName)
                {
                    setting = new CrossClusterSetting(CrossClusterSettingEnum.AllowedCrossCluster);
                }
                else if (settingSymbol.ToDisplayString() == typeof(MustBeCrossCluster).FullName)
                {
                    setting = new CrossClusterSetting(CrossClusterSettingEnum.MustBeCrossCluster);
                }
                else if (settingSymbol.ToDisplayString() == typeof(OnlyLocalCluster).FullName)
                {
                    setting = new CrossClusterSetting(CrossClusterSettingEnum.OnlyLocal);
                }
                else if (settingSymbol.ToDisplayString() == typeof(NoWrappers).FullName)
                {
                    setting = new WrapperSetting(false);
                }
                else if (settingSymbol.ToDisplayString() == typeof(ProduceWrappers).FullName)
                {
                    setting = new WrapperSetting(true);
                }
                else
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectSetting,
                        $"Incorrect setting used [{settingSymbol.ToGlobalDisplayString()}]",
                        settingSymbol.ToGlobalDisplayString()
                        );
                }

                if (settingScopes.Contains(setting.Scope))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectSetting,
                        $"Only one settings of each scope is allowed [{setting.Scope}]",
                        settingSymbol.ToGlobalDisplayString()
                        );
                }
                settingScopes.Add(setting.Scope);


                if (!setting.IsAllowedFor(scope))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectSetting,
                        $"Setting {setting.GetType().Name} is incompatible with scope {scope}"
                        );
                }

                _settings.Add(setting);
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
