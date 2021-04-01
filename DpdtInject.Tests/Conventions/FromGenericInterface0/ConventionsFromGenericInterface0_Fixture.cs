using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conventions.FromGenericInterface0
{
    [TestClass]
    public class ConventionsFromGenericInterface0_Fixture
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
                nameof(ConventionsFromGenericInterface0_Cluster.ConventionsFromGenericInterface0_ClusterTester),
                nameof(TestResources.ConventionsFromGenericInterface0_Cluster),
                TestResources.ConventionsFromGenericInterface0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
