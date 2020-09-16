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

namespace DpdtInject.Tests.Conditional.RContext.Hierarchy2Variant1
{
    public partial class ConditionalRContextHierarchy2Variant1Module : DpdtModule
    {
        public const string BMessage = "B some message";
        public const string CMessage = "C some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .When(rc => !rc.IsRoot && rc.RootFrame.RequestedType == typeof(IC))
                ;

            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .When(rc => !rc.IsRoot && rc.RootFrame.RequestedType == typeof(IB))
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("message", BMessage))
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("message", CMessage))
                ;
        }

        public class ConditionalRContextHierarchy2Variant1ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ConditionalRContextHierarchy2Variant1Module>();

                var b0 = module.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);
                Assert.AreEqual(BMessage, b0.Message);

                var bb = module.GetAll<IB>();
                Assert.IsNotNull(bb);
                Assert.AreEqual(1, bb.Count);
                Assert.AreSame(b0, bb[0]);



                var c0 = module.Get<IC>();
                Assert.IsNotNull(c0);
                Assert.IsNotNull(c0.A);
                Assert.AreEqual(CMessage, c0.Message);

                var cc = module.GetAll<IC>();
                Assert.IsNotNull(cc);
                Assert.AreEqual(1, cc.Count);
                Assert.AreSame(c0, cc[0]);

                Assert.AreNotSame(b0.A, c0.A);
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
        string Message { get; }

        IA A { get; }
    }

    public class B : IB
    {
        public string Message { get; }
        public IA A { get; }

        public B(string message, IA a)
        {
            Message = message;
            A = a;
        }
    }

    public interface IC
    {
        string Message { get; }

        IA A { get; }
    }

    public class C : IC
    {
        public string Message { get; }
        public IA A { get; }

        public C(string message, IA a)
        {
            Message = message;
            A = a;
        }
    }

}
