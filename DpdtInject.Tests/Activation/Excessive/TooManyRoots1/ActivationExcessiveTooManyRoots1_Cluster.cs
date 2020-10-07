using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Excp;
using System.Threading;
using System.Diagnostics;

namespace DpdtInject.Tests.Activation.Excessive.TooManyRoots1
{
    public partial class ActivationExcessiveTooManyRoots1_Cluster : DefaultCluster
    {
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
        }

        public class ActivationExcessiveTooManyRoots1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ActivationExcessiveTooManyRoots1_Cluster>(
                    null
                    );

                var a = cluster.Get<IA>();
                Assert.IsNotNull(a);

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

}
