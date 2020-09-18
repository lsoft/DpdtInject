using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Generic.IsRegisteredFrom
{
    public partial class GenericIsRegisteredFromModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }

        public class GenericIsRegisteredFromModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<GenericIsRegisteredFromModule>();

                var a = module.IsRegisteredFrom<IA>();
                Assert.IsTrue(a);

                var b = module.IsRegisteredFrom<IB>();
                Assert.IsTrue(b);

                var c = module.IsRegisteredFrom<IC>();
                Assert.IsFalse(c);
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
    }

    public class B : IB
    {
    }

    public interface IC
    {
    }
}
