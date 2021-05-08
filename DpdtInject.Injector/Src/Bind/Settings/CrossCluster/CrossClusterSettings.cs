using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Injector.Src.Bind.Settings.CrossCluster
{
    public abstract class CrossClusterSettings : ISetting
    {
        public const string ScopeConstant = nameof(CrossClusterSettings);
        public string Scope => ScopeConstant;

        public abstract CrossClusterSettingEnum Setting
        {
            get;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);

    }


}
