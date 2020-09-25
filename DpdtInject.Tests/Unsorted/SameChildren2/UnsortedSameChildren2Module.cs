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
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.SameChildren2
{
    public partial class UnsortedSameChildren2Module : DpdtModule
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => rc.CurrentFrame.ConstructorArgumentName == "a1")
                ;

            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => rc.CurrentFrame.ConstructorArgumentName == "a2")
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

        public class UnsortedSameChildren2Tester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<UnsortedSameChildren2Module>();

                var b0 = module.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A1);
                Assert.IsNotNull(b0.A2);
                Assert.AreNotSame(b0.A1, b0.A2);

                var b1 = module.Get<IB>();
                Assert.IsNotNull(b1);
                Assert.AreSame(b0, b1);
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
        IA A1 { get; }
        IA A2 { get; }
    }

    public class B : IB
    {
        public IA A1 { get; }
        public IA A2 { get; }

        public B(IA a1, IA a2)
        {
            A1 = a1;
            A2 = a2;
        }
    }


}
