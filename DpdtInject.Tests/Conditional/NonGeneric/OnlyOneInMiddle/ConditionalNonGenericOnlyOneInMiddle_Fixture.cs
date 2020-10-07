using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Conditional.NonGeneric.OnlyOneInMiddle
{
    [TestClass]
    public class ConditionalNonGenericOnlyOneInMiddle_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalNonGenericOnlyOneInMiddle_Cluster.ConditionalNonGenericOnlyOneInMiddle_ClusterTester),
                nameof(TestResources.ConditionalNonGenericOnlyOneInMiddle_Cluster),
                TestResources.ConditionalNonGenericOnlyOneInMiddle_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
