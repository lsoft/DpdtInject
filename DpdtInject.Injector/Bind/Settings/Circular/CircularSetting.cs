using DpdtInject.Injector.Helper;

namespace DpdtInject.Injector.Bind.Settings.Circular
{
    public abstract class CircularSetting : ISetting
    {
        public abstract bool DoCircularCheck
        {
            get;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);
    }


}
