using DpdtInject.Injector.Src.Helper;
using System;
using System.Collections.Generic;

namespace DpdtInject.Injector.Src.Bind.Settings.Constructor
{
    public abstract class ConstructorSetting : ISetting
    {
        protected readonly List<Type> _constructorArgumentsTypes;

        public const string ScopeConstant = nameof(ConstructorSetting);
        public string Scope => ScopeConstant;

        public IReadOnlyList<Type> ConstructorArgumentsTypes => _constructorArgumentsTypes;

        public abstract ConstructorSettingsEnum CheckMode
        {
            get;
        }

        public ConstructorSetting()
        {
            _constructorArgumentsTypes = new List<Type>();
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);


        protected void AddRange(params Type[] constructorArgumentsTypes)
        {
            if (constructorArgumentsTypes is null)
            {
                throw new ArgumentNullException(nameof(constructorArgumentsTypes));
            }

            _constructorArgumentsTypes.AddRange(constructorArgumentsTypes);
        }
    }
}
