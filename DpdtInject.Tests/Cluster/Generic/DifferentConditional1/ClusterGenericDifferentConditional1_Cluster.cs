using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Bind.Settings.CrossCluster;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Cluster.Generic.DifferentConditional1
{
    public partial class ClusterGenericDifferentConditional1_FirstCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .When(rt =>
                          rt.TargetType == typeof(A) && rt.ClusterDeclaredType == typeof(ClusterGenericDifferentConditional1_FirstCluster) && rt.ParentRequest.RequestedType == typeof(IA) && rt.ParentRequest.ClusterDeclaredType == typeof(ClusterGenericDifferentConditional1_LastCluster) && rt.TryGetParentTarget(out var parentTarget) && parentTarget.TargetType == typeof(B)
                    )
                ;
        }
    }

    public partial class ClusterGenericDifferentConditional1_MiddleCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
        }
    }

    public partial class ClusterGenericDifferentConditional1_LastCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .Setup<AllowedCrossCluster>()
                .When(rt =>
                          rt.IsRoot && rt.TargetType == typeof(B) && rt.ClusterDeclaredType == typeof(ClusterGenericDifferentConditional1_LastCluster) && rt.ParentRequest.RequestedType == typeof(IB)
                    )
                ;
        }
    }

    public class ClusterGenericDifferentConditional1_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var firstCluster = new FakeCluster<ClusterGenericDifferentConditional1_FirstCluster>(
                null
                );
            var middleCluster = new FakeCluster<ClusterGenericDifferentConditional1_MiddleCluster>(
                firstCluster
                );
            var lastCluster = new FakeCluster<ClusterGenericDifferentConditional1_LastCluster>(
                middleCluster
                );

            var b0 = lastCluster.Get<IB>();
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
