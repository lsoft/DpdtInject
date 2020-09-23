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

namespace DpdtInject.Tests.Activation.Excessive.TooManyRoots3
{
    public partial class ActivationExcessiveTooManyRoots3Module : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => false)
                ;

            Bind<IA>()
                .To<A2>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => false)
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class ActivationExcessiveTooManyRoots3Tester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ActivationExcessiveTooManyRoots3Module>();

                try
                {
                    var a = module.Get<IA>();

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
