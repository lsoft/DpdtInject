using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Injector.Src.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Core.Binding.Settings.Constructor
{
    public class ConstructorSetting : IDefinedSetting
    {
        private readonly List<ITypeSymbol> _constructorArgumentsTypes;

        public const string ScopeConstant = nameof(ConstructorSetting);
        public string Scope => ScopeConstant;

        public IReadOnlyList<ITypeSymbol> ConstructorArgumentsTypes => _constructorArgumentsTypes;

        public ConstructorSettingsEnum CheckMode
        {
            get;
        }

        public ConstructorSetting(
            ConstructorSettingsEnum checkMode
            )
        {
            CheckMode = checkMode;

            _constructorArgumentsTypes = new List<ITypeSymbol>();
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);


        public void AddRange(IReadOnlyList<ITypeSymbol> constructorArgumentsTypes)
        {
            if (constructorArgumentsTypes is null)
            {
                throw new ArgumentNullException(nameof(constructorArgumentsTypes));
            }

            _constructorArgumentsTypes.AddRange(constructorArgumentsTypes);
        }
    }
}
