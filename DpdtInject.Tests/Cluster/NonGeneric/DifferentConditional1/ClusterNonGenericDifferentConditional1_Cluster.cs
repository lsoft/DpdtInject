using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.NonGeneric.DifferentConditional1
{
    public partial class ClusterNonGenericDifferentConditional1_FirstCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .When(rt => 
                    rt.TargetType == typeof(A)
                    && rt.ClusterDeclaredType == typeof(ClusterNonGenericDifferentConditional1_FirstCluster)
                    && rt.ParentRequest.RequestedType == typeof(IA)
                    && rt.ParentRequest.ClusterDeclaredType == typeof(ClusterNonGenericDifferentConditional1_LastCluster)
                    && rt.TryGetParentTarget(out var parentTarget)
                    && parentTarget.TargetType == typeof(B)
                    )
                ;
        }
    }

    public partial class ClusterNonGenericDifferentConditional1_MiddleCluster : DefaultCluster
    {
        public override void Load()
        {
            
        }
    }

    public partial class ClusterNonGenericDifferentConditional1_LastCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .When(rt => 
                    rt.IsRoot 
                    && rt.TargetType == typeof(B)
                    && rt.ClusterDeclaredType == typeof(ClusterNonGenericDifferentConditional1_LastCluster)
                    && rt.ParentRequest.RequestedType == typeof(IB)
                    )
                ;
        }
    }

    public class ClusterNonGenericDifferentConditional1_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var firstCluster = new FakeCluster<ClusterNonGenericDifferentConditional1_FirstCluster>(
                null
                );
            var middleCluster = new FakeCluster<ClusterNonGenericDifferentConditional1_MiddleCluster>(
                firstCluster
                );
            var lastCluster = new FakeCluster<ClusterNonGenericDifferentConditional1_LastCluster>(
                middleCluster
                );

            var b0 = (IB)lastCluster.Get(typeof(IB));
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
