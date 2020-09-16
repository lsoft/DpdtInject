using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Transient.SingleObjectWithArgument
{

    [TestClass]
    public class TransientSingleObjectWithArgumentFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(TransientSingleObjectWithArgumentModule.TransientSingleObjectWithArgumentModuleTester),
                nameof(TestResources.TransientSingleObjectWithArgumentModule),
                TestResources.TransientSingleObjectWithArgumentModule
                );

            preparation.Check();
        }

    }
}
