using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src.Bind.Settings.CrossCluster;
using DpdtInject.Injector.Src.Bind.Settings.Wrapper;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Cluster.NonGeneric.DifferentFunc
{
    public partial class ClusterNonGenericDifferentFunc_RootCluster : DefaultCluster
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

    public partial class ClusterNonGenericDifferentFunc_ChildCluster : DefaultCluster
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

    public class ClusterNonGenericDifferentFunc_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterNonGenericDifferentFunc_RootCluster>(
                null
                );
            var childCluster = new FakeCluster<ClusterNonGenericDifferentFunc_ChildCluster>(
                rootCluster
                );

            var a = (IA) rootCluster.Get(typeof(IA));
            Assert.IsNotNull(a);

            var b0 = (IB) childCluster.Get(typeof(IB));
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
