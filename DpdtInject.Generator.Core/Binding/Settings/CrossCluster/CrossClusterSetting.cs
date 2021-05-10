using DpdtInject.Generator.Core.Binding.Settings.Constructor;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Generator.Core.Binding.Settings.CrossCluster
{
    public class CrossClusterSetting : IDefinedSetting
    {
        public const string ScopeConstant = nameof(CrossClusterSetting);
        public string Scope => ScopeConstant;

        public CrossClusterSettingEnum Setting
        {
            get;
        }

        public CrossClusterSetting(CrossClusterSettingEnum setting)
        {
            Setting = setting;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);

    }


}
