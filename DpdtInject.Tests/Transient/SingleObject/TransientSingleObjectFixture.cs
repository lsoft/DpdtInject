using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Transient.SingleObject
{

    [TestClass]
    public class TransientSingleObjectFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(TransientSingleObjectModule.TransientSingleObjectModuleTester),
                nameof(TestResources.TransientSingleObjectModule),
                TestResources.TransientSingleObjectModule
                );

            preparation.Check();
        }

    }
}
