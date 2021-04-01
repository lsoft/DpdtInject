using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conventions.FromGenericInterface1
{
    [TestClass]
    public class ConventionsFromGenericInterface1_Fixture
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
                nameof(ConventionsFromGenericInterface1_Cluster.ConventionsFromGenericInterface1_ClusterTester),
                nameof(TestResources.ConventionsFromGenericInterface1_Cluster),
                TestResources.ConventionsFromGenericInterface1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
