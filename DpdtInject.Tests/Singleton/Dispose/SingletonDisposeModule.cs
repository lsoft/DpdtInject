using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Singleton.Dispose
{
    public partial class SingletonDisposeModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class SingletonDisposeModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<SingletonDisposeModule>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

                Assert.AreEqual(0, A.DisposeCount);

                module.Dispose();

                Assert.AreEqual(1, A.DisposeCount);
            }
        }

    }


    public interface IA
    {
    }

    public class A : IA, IDisposable
    {
        public static int DisposeCount = 0;

        public void Dispose()
        {
            DisposeCount++;
        }
    }

}
