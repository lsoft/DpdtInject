using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.Fast.Hierarchy2
{
    public partial class ScopeSingletonFastHierarchy2_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                ;
        }

        public class ScopeSingletonFastHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeSingletonFastHierarchy2_Cluster>(
                    null
                    );

                var a = cluster.GetFast(default(IA));
                Assert.IsNotNull(a);

                var b0 = cluster.GetFast(default(IB));
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);

                Assert.AreSame(a, b0.A);

                var b1 = cluster.GetFast(default(IB));
                Assert.IsNotNull(b1);
                Assert.IsNotNull(b1.A);
                Assert.AreSame(b0, b1);
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
