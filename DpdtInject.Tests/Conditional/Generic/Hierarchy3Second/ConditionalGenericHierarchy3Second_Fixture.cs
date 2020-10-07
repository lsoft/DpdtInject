using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Conditional.Generic.Hierarchy3Second
{
    [TestClass]
    public class ConditionalGenericHierarchy3Second_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalGenericHierarchy3Second_Cluster.ConditionalGenericHierarchy3Second_ClusterTester),
                nameof(TestResources.ConditionalGenericHierarchy3Second_Cluster),
                TestResources.ConditionalGenericHierarchy3Second_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
