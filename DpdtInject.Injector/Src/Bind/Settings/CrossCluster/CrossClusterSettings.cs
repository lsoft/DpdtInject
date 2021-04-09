using DpdtInject.Injector.Helper;

namespace DpdtInject.Injector.Bind.Settings.CrossCluster
{
    public abstract class CrossClusterSettings : ISetting
    {

        public abstract CrossClusterSettingEnum Setting
        {
            get;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);

    }


}
