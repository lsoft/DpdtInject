using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src.Bind.Settings.CrossCluster;
using DpdtInject.Injector.Src.Bind.Settings.Wrapper;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Cluster.Generic.DifferentFunc
{
    public partial class ClusterGenericDifferentFunc_RootCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .Setup<ProduceWrappers>()
                ;
        }
    }

    public partial class ClusterGenericDifferentFunc_ChildCluster : DefaultCluster
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

    public class ClusterGenericDifferentFunc_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterGenericDifferentFunc_RootCluster>(
                null
                );
            var childCluster = new FakeCluster<ClusterGenericDifferentFunc_ChildCluster>(
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
            Func<IA> af
            )
        {
            A = af();
        }
    }
}
