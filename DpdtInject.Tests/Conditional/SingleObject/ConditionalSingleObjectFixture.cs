using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Singleton.Conditional
{

    [TestClass]
    public class ConditionalSingleObjectFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalSingleObjectModule.ConditionalSingleObjectModuleTester),
                nameof(TestResources.ConditionalSingleObjectModule),
                TestResources.ConditionalSingleObjectModule
                );

            preparation.Check();
        }

    }
}
