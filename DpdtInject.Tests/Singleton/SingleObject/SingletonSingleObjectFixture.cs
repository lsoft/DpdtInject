using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Singleton.SingleObject
{

    [TestClass]
    public class SingletonSingleObjectFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(SingletonSingleObjectModule.SingletonSingleObjectModuleTester),
                nameof(TestResources.SingletonSingleObjectModule),
                TestResources.SingletonSingleObjectModule
                );

            preparation.Check();
        }

    }
}
