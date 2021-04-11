using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Injector.Src.Bind.Settings.CrossCluster
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
