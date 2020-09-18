using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Transient.Func.Zero
{
    public partial class TransientFuncZeroModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;
        }

        public class TransientFuncZeroModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<TransientFuncZeroModule>();

                var af0 = module.GetFunc<IA>();
                var af1 = module.GetFunc<IA>();
                Assert.AreSame(af0, af1);

                var a00 = af0();
                var a01 = af0();
                Assert.AreNotSame(a00, a01);

                var a10 = af1();
                var a11 = af1();
                Assert.AreNotSame(a10, a11);
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
