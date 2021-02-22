using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            }
        }
    }


    public interface IA
    {
    }

    public class A : IA
    {
    }

    public interface IAFactory
    {
        IA Create();
    }

    public partial class AFactory : IFakeFactory<IAFactory>
    {
    }
}
