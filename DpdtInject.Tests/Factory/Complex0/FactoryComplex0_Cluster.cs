using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Bind;

namespace DpdtInject.Tests.Factory.Complex0
{
    public partial class FactoryComplex0_Cluster : DefaultCluster
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

            Bind<C>()
                .To<C>()
                .WithSingletonScope()
                .When(rt => rt.ParentRequest.ConstructorArgumentName == "c")
                ;
        }

        public class FactoryComplex0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<FactoryComplex0_Cluster>(
                    null
                    );

                var af = cluster.Get<IAFactory>();
                Assert.IsNotNull(af);

                var a0 = af.Create(A);
                Assert.IsNotNull(a0);
                Assert.AreEqual(a0.A, A);
                Assert.AreEqual(a0.B, B);
                Assert.IsNotNull(a0.C);

                var a1 = af.Create(A);
                Assert.IsNotNull(a1);
                Assert.AreEqual(a1.A, A);
                Assert.AreEqual(a1.B, B);
                Assert.IsNotNull(a1.C);

                Assert.AreNotSame(a0, a1);

                Assert.AreSame(a0.C, a1.C);
            }
        }
    }


    public class C
    {
    }

    public interface IA
    {
        string A
        {
            get;
        }

        int B
        {
            get;
        }

        C C
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

        public int B
        {
            get;
        }

        public C C
        {
            get;
        }


        public A0(
            string a,
            int b,
            C c
            )
        {
            A = a;
            B = b;
            C = c;
        }
    }

    public interface IAFactory
    {
        IA Create(
            string a
            );
    }

    public partial class AFactory : IFakeFactory<IAFactory>
    {
    }
}
