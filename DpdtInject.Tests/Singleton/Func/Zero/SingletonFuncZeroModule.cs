using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Singleton.Func.Zero
{
    public partial class SingletonFuncZeroModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class SingletonFuncZeroModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<SingletonFuncZeroModule>();

                var af0 = module.GetFunc<IA>();
                var af1 = module.GetFunc<IA>();
                Assert.AreSame(af0, af1);

                var a00 = af0();
                var a01 = af0();
                Assert.AreSame(a00, a01);

                var a10 = af1();
                var a11 = af1();
                Assert.AreSame(a10, a11);
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
