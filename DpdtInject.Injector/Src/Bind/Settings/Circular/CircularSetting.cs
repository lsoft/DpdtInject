using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Injector.Src.Bind.Settings.Circular
{
    public abstract class CircularSetting : ISetting
    {
        public const string ScopeConstant = nameof(CircularSetting);
        public string Scope => ScopeConstant;

        public abstract bool DoCircularCheck
        {
            get;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);
    }


}
