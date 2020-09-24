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

namespace DpdtInject.Tests.Activation.Excessive.TooManyChildren0
{
    public partial class ActivationExcessiveTooManyChildren0Module : DpdtModule
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => true)
                ;

            Bind<IA>()
                .To<A2>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => false)
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class ActivationExcessiveTooManyChildren0Tester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ActivationExcessiveTooManyChildren0Module>();

                var b = module.Get<IB>();
                Assert.IsNotNull(b);
                Assert.IsNotNull(b.A);

                Assert.AreEqual(1, B.ActivationCount, "B.ActivationCount");
                Assert.AreEqual(1, A1.ActivationCount, "A1.ActivationCount");
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
