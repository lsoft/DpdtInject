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
using NuGet.Frameworks;

namespace DpdtInject.Tests.Transient.Hierarchy3
{
    public partial class TransientHierarchy3Module : DpdtModule
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

            Bind<IC>()
                .To<C>()
                .WithTransientScope()
                ;
        }

        public class TransientHierarchy3ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<TransientHierarchy3Module>();

                var c0 = module.Get<IC>();
                Assert.IsNotNull(c0);
                Assert.IsNotNull(c0.B);
                Assert.AreEqual(Message, c0.B.Message);
                Assert.IsNotNull(c0.B.A);

                var c1 = module.Get<IC>();
                Assert.IsNotNull(c1);
                Assert.IsNotNull(c1.B);
                Assert.IsNotNull(c0.B.A);

                Assert.AreNotSame(c0, c1);

                var cc = module.GetAll<IC>();
                Assert.IsNotNull(cc);
                Assert.AreEqual(1, cc.Count);
                Assert.AreNotSame(c0, cc[0]);
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
        IB B { get; }
    }


    public class C : IC
    {
        public IB B { get; }

        public C(IB b)
        {
            B = b;
        }
    }

}
