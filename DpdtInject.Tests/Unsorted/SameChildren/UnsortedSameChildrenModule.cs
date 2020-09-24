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

namespace DpdtInject.Tests.Unsorted.SameChildren
{
    public partial class UnsortedSameChildrenModule : DpdtModule
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => rc.CurrentFrame.ConstructorArgumentName == "a1")
                ;

            Bind<IA>()
                .To<A2>()
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

        public class UnsortedSameChildrenTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<UnsortedSameChildrenModule>();

                var b = module.Get<IB>();
                Assert.IsNotNull(b);
                Assert.IsNotNull(b.A1);
                Assert.IsNotNull(b.A2);

                Assert.AreEqual(typeof(A1), b.A1.GetType());
                Assert.AreEqual(typeof(A2), b.A2.GetType());
            }
        }

    }


    public interface IA
    {
    }

    public class A2 : IA
    {
    }
    public class A1 : IA
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
