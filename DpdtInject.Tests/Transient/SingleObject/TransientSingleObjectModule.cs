using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Transient.SingleObject
{
    public partial class TransientSingleObjectModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class TransientSingleObjectModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<TransientSingleObjectModule>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

                var a1 = module.Get<IA>();
                Assert.IsNotNull(a1);

                Assert.AreNotSame(a0, a1);

                var aa = module.GetAll<IA>();
                Assert.IsNotNull(aa);
                Assert.AreEqual(1, aa.Count);
                Assert.AreNotSame(a0, aa[0]);
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
