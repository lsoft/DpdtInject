using DpdtInject.Injector.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Bind.Settings
{
    public interface ISetting
    {
        bool IsAllowedFor(BindScopeEnum scope);
    }


    public interface ISettingsProvider
    {
        bool IsSetup<T>()
             where T : class, ISetting;

        bool TryGetSettingInScope<TScope>([NotNullWhen(true)]  out TScope? setting)
             where TScope : class, ISetting;

    }

    public enum CrossClusterSettingEnum
    {
        OnlyLocal,
        AllowedCrossCluster,
        MustBeCrossCluster
    }

    public abstract class CrossClusterSettings : ISetting
    {

        public abstract CrossClusterSettingEnum Setting
        {
            get;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);

    }

    public class OnlyLocalCluster : CrossClusterSettings
    {
        public override CrossClusterSettingEnum Setting => CrossClusterSettingEnum.OnlyLocal;
    }

    public class AllowedCrossCluster : CrossClusterSettings
    {
        public override CrossClusterSettingEnum Setting => CrossClusterSettingEnum.AllowedCrossCluster;
    }

    public class MustBeCrossCluster : CrossClusterSettings
    {
        public override CrossClusterSettingEnum Setting => CrossClusterSettingEnum.MustBeCrossCluster;
    }




    public abstract class WrappersSettings : ISetting
    {
        public abstract bool DoProduceWrappers
        {
            get;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => true;
    }

    public class NoWrappers : WrappersSettings
    {
        public override bool DoProduceWrappers => false;
    }

    public class ProduceWrappers : WrappersSettings
    {
        public override bool DoProduceWrappers => true;
    }




    public abstract class CircularSetting : ISetting
    {
        public abstract bool DoCircularCheck
        {
            get;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => scope.In(BindScopeEnum.Singleton, BindScopeEnum.Transient, BindScopeEnum.Custom);
    }

    public class SuppressCircularCheck : CircularSetting
    {
        public override bool DoCircularCheck => false;
    }

    public class PerformCircularCheck : CircularSetting
    {
        public override bool DoCircularCheck => true;
    }


}
