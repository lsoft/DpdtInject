using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Activation.Excessive.TooManyRoots3
{
    public partial class ActivationExcessiveTooManyRoots3_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                .When(rc => false)
                ;

            Bind<IA>()
                .To<A2>()
                .WithSingletonScope()
                .When(rc => false)
                ;
        }

        public class ActivationExcessiveTooManyRoots3_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ActivationExcessiveTooManyRoots3_Cluster>(
                    null
                    );

                try
                {
                    var a = cluster.Get<IA>();

                    Assert.Fail("this line should never be executed");
                }
                catch (DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IA).FullName)
                {
                    //it's OK, this test is gree
                }

                Assert.AreEqual(0, A1.ActivationCount, "A1.ActivationCount");
                Assert.AreEqual(0, A2.ActivationCount, "A2.ActivationCount");
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
}
