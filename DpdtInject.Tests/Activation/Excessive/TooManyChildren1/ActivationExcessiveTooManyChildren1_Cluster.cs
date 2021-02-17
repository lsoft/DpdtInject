using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace DpdtInject.Tests.Activation.Excessive.TooManyChildren1
{
    public partial class ActivationExcessiveTooManyChildren1_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                .When(rc => false)
                ;

            Bind<IA>()
                .To<A2>()
                .WithSingletonScope()
                .When(rc => true)
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                ;
        }

        public class ActivationExcessiveTooManyChildren1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ActivationExcessiveTooManyChildren1_Cluster>(
                    null
                    );

                var b = cluster.Get<IB>();
                Assert.IsNotNull(b);
                Assert.IsNotNull(b.A);

                Assert.AreEqual(1, B.ActivationCount, "B.ActivationCount");
                Assert.AreEqual(0, A1.ActivationCount, "A1.ActivationCount");
                Assert.AreEqual(1, A2.ActivationCount, "A2.ActivationCount");
            }
        }

    }


    public interface IA
    {
    }

    public class A2 : IA
    {
        public static long ActivationCount = 0L;

        public A2()
        {
            Interlocked.Increment(ref ActivationCount);
        }
    }

    public class A1 : IA
    {
        public static long ActivationCount = 0L;

        public A1()
        {
            Interlocked.Increment(ref ActivationCount);
        }
    }

    public interface IB
    {
        IA A { get; }
    }

    public class B : IB
    {
        public static long ActivationCount = 0L;

        public IA A { get; }

        public B(IA a)
        {
            Interlocked.Increment(ref ActivationCount);

            A = a;
        }
    }


}
