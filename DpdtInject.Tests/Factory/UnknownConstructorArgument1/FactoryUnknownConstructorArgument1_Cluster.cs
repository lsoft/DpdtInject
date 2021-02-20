using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Bind;

namespace DpdtInject.Tests.Factory.UnknownConstructorArgument1
{
    public partial class FactoryUnknownConstructorArgument1_Cluster : DefaultCluster
    {
        public const string A = "0";
        public const int B = 1;
        public const long C = 2L;

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IAFactory>()
                .ToIsolatedFactory<AFactory>()
                .WithPayload<A0>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("b", B))
                ;
        }

        public class FactoryUnknownConstructorArgument1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<FactoryUnknownConstructorArgument1_Cluster>(
                    null
                    );

                var af = cluster.Get<IAFactory>();
                Assert.IsNotNull(af);

                var a0 = af.Create(A);
                Assert.IsNotNull(a0);
                Assert.AreEqual(a0.A, A);
                Assert.AreEqual(a0.B, B);

                var a1 = af.Create(C);
                Assert.IsNotNull(a1);
                Assert.AreEqual(a1.B, B);
                Assert.AreEqual(a1.C, C);

                Assert.AreNotSame(a0, a1);
            }
        }
    }


    public interface IA
    {
        string A
        {
            get;
        }

        long C
        {
            get;
        }

        int B
        {
            get;
        }
    }

    public class A0 : IA
    {
        public string A
        {
            get;
        }

        public long C
        {
            get;
        }

        public int B
        {
            get;
        }

        public A0(
            long c,
            int b
            )
        {
            C = c;
            B = b;
        }

        public A0(
            string a,
            int b
            )
        {
            A = a;
            B = b;
        }
    }

    public interface IAFactory
    {
        IA Create(
            long c
            );

        IA Create(
            string a
            );
    }

    public partial class AFactory : IFakeFactory<IAFactory>
    {
    }
}
