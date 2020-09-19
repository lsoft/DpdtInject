using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Func.NonGeneric.Transient.SingleObject
{
    public partial class FuncNonGenericTransientSingleObjectModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;
        }

        public class FuncNonGenericTransientSingleObjectModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<FuncNonGenericTransientSingleObjectModule>();

                var a0 = (IA)module.Get(typeof(IA));
                Assert.IsNotNull(a0);

                var af = (Func<IA>)module.Get(typeof(Func<IA>));
                Assert.IsNotNull(af);
                
                var a1 = af();
                Assert.IsNotNull(a1);
                Assert.AreNotSame(a0, a1);
            }
        }

    }


    public interface IA
    {
    }

    public class A : IA
    {
    }

}
