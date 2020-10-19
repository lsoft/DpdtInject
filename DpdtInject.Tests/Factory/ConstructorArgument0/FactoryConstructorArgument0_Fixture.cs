using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Factory.ConstructorArgument0
{
    [TestClass]
    public class FactoryConstructorArgument0_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FactoryConstructorArgument0_Cluster.FactoryConstructorArgument0_ClusterTester),
                nameof(TestResources.FactoryConstructorArgument0_Cluster),
                TestResources.FactoryConstructorArgument0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
