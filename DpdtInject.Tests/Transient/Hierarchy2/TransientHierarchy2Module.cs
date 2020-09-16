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

namespace DpdtInject.Tests.Transient.Hierarchy2
{
    public partial class TransientHierarchy2Module : DpdtModule
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .Configure(new ConstructorArgument("message", Message))
                ;
        }

        public class TransientHierarchy2ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<TransientHierarchy2Module>();

                var b0 = module.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);
                Assert.AreEqual(Message, b0.Message);

                var b1 = module.Get<IB>();
                Assert.IsNotNull(b1);
                Assert.IsNotNull(b1.A);

                Assert.AreNotSame(b0, b1);

                var bb = module.GetAll<IB>();
                Assert.IsNotNull(bb);
                Assert.AreEqual(1, bb.Count);
                Assert.AreNotSame(b0, bb[0]);
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

}
