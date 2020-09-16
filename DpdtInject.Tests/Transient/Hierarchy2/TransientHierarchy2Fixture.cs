using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Transient.Hierarchy2
{

    [TestClass]
    public class TransientHierarchy2Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(TransientHierarchy2Module.TransientHierarchy2ModuleTester),
                nameof(TestResources.TransientHierarchy2Module),
                TestResources.TransientHierarchy2Module
                );

            preparation.Check();
        }

    }
}
