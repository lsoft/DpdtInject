using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Factory.Simple0
{
    public partial class FactorySimple0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IAFactory>()
                .ToIsolatedFactory<AFactory>()
                .WithPayload<A>()
                .WithSingletonScope()
                ;
        }

        public class FactorySimple0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<FactorySimple0_Cluster>(
                    null
                    );

                var af = cluster.Get<IAFactory>();
                Assert.IsNotNull(af);

                var a0 = af.Create();
                Assert.IsNotNull(a0);

                var a1 = af.Create();
                Assert.IsNotNull(a1);

                Assert.AreNotSame(a0, a1);

                var a2 = af.Create(0);
                Assert.IsNotNull(a2);

                var unused = 0L;
                var a3 = af.Create(ref unused);
                Assert.IsNotNull(a3);
            }
        }
    }


    public interface IA
    {
    }

    public class A : IA
    {
        public A()
        {
        }

        public A(int unused)
        {
        }

        public A(ref long unused)
        {
        }
    }

    public interface IAFactory
    {
        IA Create();

        IA Create(in int unused);

        IA Create(ref long unused);
    }

    public partial class AFactory : IFakeFactory<IAFactory>
    {
    }
}
