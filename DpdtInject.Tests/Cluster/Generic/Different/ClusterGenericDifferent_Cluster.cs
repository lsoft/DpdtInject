using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Bind.Settings.CrossCluster;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Cluster.Generic.Different
{
    public partial class ClusterGenericDifferent_RootCluster : DefaultCluster
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

    public partial class ClusterGenericDifferent_ChildCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .Setup<AllowedCrossCluster>()
                ;
        }
    }

    public class ClusterGenericDifferent_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterGenericDifferent_RootCluster>(
                null
                );
            var childCluster = new FakeCluster<ClusterGenericDifferent_ChildCluster>(
                rootCluster
                );

            var a = rootCluster.Get<IA>();
            Assert.IsNotNull(a);

            var b0 = childCluster.Get<IB>();
            Assert.IsNotNull(b0);
            Assert.IsNotNull(b0.A);
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
