using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Func.Generic.Singleton.SingleObject
{
    public partial class FuncGenericSingletonSingleObjectModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }
        public class FuncGenericSingletonSingleObjectModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<FuncGenericSingletonSingleObjectModule>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

                var af = module.Get<Func<IA>>();
                Assert.IsNotNull(af);
                
                var a1 = af();
                Assert.IsNotNull(a1);
                Assert.AreSame(a0, a1);
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
