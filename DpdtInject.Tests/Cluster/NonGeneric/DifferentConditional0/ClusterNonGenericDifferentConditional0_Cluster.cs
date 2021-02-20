using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.NonGeneric.DifferentConditional0
{
    public partial class ClusterNonGenericDifferentConditional0_FirstCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .When(rt =>
                          rt.TargetType == typeof(A) && rt.ClusterDeclaredType == typeof(ClusterNonGenericDifferentConditional0_FirstCluster) && rt.ParentRequest.RequestedType == typeof(IA) && rt.ParentRequest.ClusterDeclaredType == typeof(ClusterNonGenericDifferentConditional0_LastCluster) && rt.TryGetParentTarget(out var parentTarget) && parentTarget.TargetType == typeof(B)
                    )
                ;
        }
    }

    public partial class ClusterNonGenericDifferentConditional0_LastCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .When(rt =>
                          rt.IsRoot && rt.TargetType == typeof(B) && rt.ClusterDeclaredType == typeof(ClusterNonGenericDifferentConditional0_LastCluster) && rt.ParentRequest.RequestedType == typeof(IB)
                    )
                ;
        }
    }

    public class ClusterNonGenericDifferentConditional0_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterNonGenericDifferentConditional0_FirstCluster>(
                null
                );
            var childCluster = new FakeCluster<ClusterNonGenericDifferentConditional0_LastCluster>(
                rootCluster
                );

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
            IA a
            )
        {
            A = a;
        }
    }
}
