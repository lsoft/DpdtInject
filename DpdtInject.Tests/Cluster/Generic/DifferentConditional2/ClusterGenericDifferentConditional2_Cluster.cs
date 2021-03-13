using DpdtInject.Injector;
using DpdtInject.Injector.Bind.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.Generic.DifferentConditional2
{
    public partial class ClusterGenericDifferentConditional2_RootCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .When(rt => rt.ParentTarget.TargetType == typeof(B))
                ;
        }
    }

    public partial class ClusterGenericDifferentConditional2_ChildCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .Setup<AllowedCrossCluster>()
                .When(rt => rt.ParentTarget.TargetType == typeof(C))
                ;

            Bind<IC>()
                .To<C>()
                .WithTransientScope()
                ;
        }
    }


    public class ClusterGenericDifferentConditional2_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterGenericDifferentConditional2_RootCluster>(
                null
                );
            var childCluster = new FakeCluster<ClusterGenericDifferentConditional2_ChildCluster>(
                rootCluster
                );

            var c = childCluster.Get<IC>();
            Assert.IsNotNull(c);
            Assert.IsNotNull(c.B);
            Assert.IsNotNull(c.B.A);
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

    public interface IC
    {
        IB B
        {
            get;
        }
    }

    public class C : IC
    {
        public IB B
        {
            get;
        }

        public C(
            IB b
            )
        {
            B = b;
        }
    }
}
