using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conventions.From2Interfaces0
{
    [TestClass]
    public class ConventionsFrom2Interfaces0_Fixture
    {
        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConventionsFrom2Interfaces0_Cluster.ConventionsFrom2Interfaces0_ClusterTester),
                nameof(TestResources.ConventionsFrom2Interfaces0_Cluster),
                TestResources.ConventionsFrom2Interfaces0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
