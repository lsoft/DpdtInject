using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Singleton.Dispose
{

    [TestClass]
    public class SingletonDisposeFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(SingletonDisposeModule.SingletonDisposeModuleTester),
                nameof(TestResources.SingletonDisposeModule),
                TestResources.SingletonDisposeModule
                );

            preparation.Check();
        }

    }
}
