using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Singleton.SingleObject
{
    public partial class SingletonSingleObjectModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class SingletonSingleObjectModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<SingletonSingleObjectModule>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

                var a1 = module.Get<IA>();
                Assert.IsNotNull(a1);

                Assert.AreSame(a0, a1);

                var aa = module.GetAll<IA>();
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
