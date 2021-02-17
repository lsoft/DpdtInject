using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.NonGeneric.Hierarchy2
{
    public partial class ScopeSingletonNonGenericHierarchy2_Cluster : DefaultCluster
    {
        public override void Load()
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

        public class ScopeSingletonNonGenericHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeSingletonNonGenericHierarchy2_Cluster>(
                    null
                    );

                var a = (IA)cluster.Get(typeof(IA));
                Assert.IsNotNull(a);

                var b0 = (IB)cluster.Get(typeof(IB));
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);

                Assert.AreSame(a, b0.A);

                var b1 = (IB)cluster.Get(typeof(IB));
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
