using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Wrapper.Func.Singleton
{
    [TestClass]
    public class WrapperFuncSingleton_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(WrapperFuncSingleton_Cluster.WrapperFuncSingleton_ClusterTester),
                nameof(TestResources.WrapperFuncSingleton_Cluster),
                TestResources.WrapperFuncSingleton_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
