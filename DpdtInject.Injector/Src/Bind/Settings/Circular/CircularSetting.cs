using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Injector.Src.Bind.Settings.Circular
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
