using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.NonGeneric.SingleObject
{
    public partial class NonGenericSingleObjectModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class NonGenericSingleObjectModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<NonGenericSingleObjectModule>();

                var a0 = module.Get(typeof(IA));
                Assert.IsNotNull(a0);
                Assert.AreEqual(typeof(A), a0.GetType());

                var a1 = module.Get(typeof(IA));
                Assert.IsNotNull(a1);
                Assert.AreEqual(typeof(A), a1.GetType());

                Assert.AreSame(a0, a1);

                var aa = module.GetAll(typeof(IA));
                Assert.IsNotNull(aa);
                Assert.AreEqual(1, aa.Count);
                Assert.AreSame(a0, aa[0]);
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
