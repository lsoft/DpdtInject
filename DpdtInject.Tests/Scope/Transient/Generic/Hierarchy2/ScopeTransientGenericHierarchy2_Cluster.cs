using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Scope.Transient.Generic.Hierarchy2
{
    public partial class ScopeTransientGenericHierarchy2_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }

        public class ScopeTransientGenericHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeTransientGenericHierarchy2_Cluster>(
                    null
                    );

                var a = cluster.Get<IA>();
                Assert.IsNotNull(a);

                var b0 = cluster.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);

                Assert.AreNotSame(a, b0.A);

                var b1 = cluster.Get<IB>();
                Assert.IsNotNull(b1);
                Assert.IsNotNull(b1.A);
                Assert.AreNotSame(b0, b1);
            }
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
