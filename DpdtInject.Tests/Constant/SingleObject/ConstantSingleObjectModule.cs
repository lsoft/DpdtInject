using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Constant.SingleObject
{
    public partial class ConstantSingleObjectModule : DpdtModule
    {
        public static A ConstantA = new A();

        public override void Load()
        {
            Bind<IA>()
                .WithConstScope(ConstantA)
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class ConstantSingleObjectModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ConstantSingleObjectModule>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);
                Assert.AreSame(ConstantA, a0);

                var a1 = module.Get<IA>();
                Assert.IsNotNull(a1);
                Assert.AreSame(ConstantA, a1);

                var aa = module.GetAll<IA>();
                Assert.IsNotNull(aa);
                Assert.AreEqual(1, aa.Count);
                Assert.AreSame(ConstantA, aa[0]);
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
