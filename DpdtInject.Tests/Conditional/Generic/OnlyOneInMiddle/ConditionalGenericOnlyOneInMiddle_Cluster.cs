using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.Generic.OnlyOneInMiddle
{
    public partial class ConditionalGenericOnlyOneInMiddle_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .When(rc => false)
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .When(rc => true)
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                ;
        }

        public class ConditionalGenericOnlyOneInMiddle_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConditionalGenericOnlyOneInMiddle_Cluster>(
                    null
                    );

                var a0 = cluster.Get<IA>();
                Assert.IsNotNull(a0);

                var b0 = cluster.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.AreSame(a0, b0.A);

                var c0 = cluster.Get<IC>();
                Assert.IsNotNull(c0);
                Assert.AreSame(b0, c0.B);
                Assert.AreSame(a0, c0.B.A);

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

    public interface IC
    {
        IB B { get; }
    }

    public class C : IC
    {
        public IB B { get; }

        public C(IB b)
        {
            B = b;
        }
    }



}
