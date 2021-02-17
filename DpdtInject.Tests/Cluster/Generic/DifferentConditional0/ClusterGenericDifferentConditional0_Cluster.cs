using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.Generic.DifferentConditional0
{
    public partial class ClusterGenericDifferentConditional0_FirstCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .When(rt => 
                    rt.TargetType == typeof(A)
                    && rt.ClusterDeclaredType == typeof(ClusterGenericDifferentConditional0_FirstCluster)
                    && rt.ParentRequest.RequestedType == typeof(IA)
                    && rt.ParentRequest.ClusterDeclaredType == typeof(ClusterGenericDifferentConditional0_LastCluster)
                    && rt.TryGetParentTarget(out var parentTarget)
                    && parentTarget.TargetType == typeof(B)
                    )
                ;
        }
    }

    public partial class ClusterGenericDifferentConditional0_LastCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .When(rt => 
                    rt.IsRoot 
                    && rt.TargetType == typeof(B)
                    && rt.ClusterDeclaredType == typeof(ClusterGenericDifferentConditional0_LastCluster)
                    && rt.ParentRequest.RequestedType == typeof(IB)
                    )
                ;
        }
    }

    public class ClusterGenericDifferentConditional0_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterGenericDifferentConditional0_FirstCluster>(
                null
                );
            var childCluster = new FakeCluster<ClusterGenericDifferentConditional0_LastCluster>(
                rootCluster
                );

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
        IA A { get; }
    }

    public class B : IB
    {
        public IA A { get; }

        public B(IA a)
        {
            A = a;
        }

    }
}
