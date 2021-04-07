using DpdtInject.Injector;
using DpdtInject.Injector.Bind.Settings.CrossCluster;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Settings.CrossCluster.MustBeCrossCluster0
{
    public partial class SettingsCrossClusterMustBeCrossCluster0_RootCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;
        }
    }

    public partial class SettingsCrossClusterMustBeCrossCluster0_ChildCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .When(rt => false)
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .Setup<MustBeCrossCluster>()
                ;
        }
    }

    public class SettingsCrossClusterMustBeCrossCluster0_ClusterTester
    {
        public void PerformClusterTesting()
        {
            //no need to have code here, there was compilation error
        }
    }


    public interface IA
    {
    }

    public class A : IA
    {
    }

    public interface IB
    {
        IA A
        {
            get;
        }
    }

    public class B : IB
    {
        public IA A
        {
            get;
        }

        public B(
            IA a
            )
        {
            A = a;
        }
    }
}
