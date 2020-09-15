using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Singleton.SingleObjectWithArgument
{

    [TestClass]
    public class SingletonSingleObjectWithArgumentFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(SingletonSingleObjectWithArgumentModule.SingletonSingleObjectWithArgumentModuleTester),
                nameof(TestResources.SingletonSingleObjectWithArgumentModule),
                TestResources.SingletonSingleObjectWithArgumentModule
                );

            preparation.Check();
        }

    }
}
