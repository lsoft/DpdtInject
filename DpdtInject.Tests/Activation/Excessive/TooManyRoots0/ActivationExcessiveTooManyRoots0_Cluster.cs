using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Excp;
using System.Threading;

namespace DpdtInject.Tests.Activation.Excessive.TooManyRoots0
{
    public partial class ActivationExcessiveTooManyRoots0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                .When(rc => true)
                ;

            Bind<IA>()
                .To<A2>()
                .WithSingletonScope()
                .When(rc => true)
                ;
        }


        public class ActivationExcessiveTooManyRoots0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ActivationExcessiveTooManyRoots0_Cluster>(
                    null
                    );

                try
                {
                    var a = cluster.Get<IA>();

                    Assert.Fail("this line should never be executed");
                }
                catch (DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.DuplicateBinding && excp.AdditionalArgument == typeof(IA).FullName)
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
