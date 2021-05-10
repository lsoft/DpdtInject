using DpdtInject.Generator.Core.Binding.Settings.Constructor;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Generator.Core.Binding.Settings.Circular
{
    public class CircularSetting : IDefinedSetting
    {
        public const string ScopeConstant = nameof(CircularSetting);
        public string Scope => ScopeConstant;

        public bool DoCircularCheck
        {
            get;
        }

        public CircularSetting(
            bool doCircularCheck
            )
        {
            DoCircularCheck = doCircularCheck;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);
    }


}
