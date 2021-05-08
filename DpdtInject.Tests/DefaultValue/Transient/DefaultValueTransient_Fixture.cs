using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.DefaultValue.Transient
{
    [TestClass]
    public class DefaultValueTransient_Fixture
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
                nameof(DefaultValueTransient_Cluster.DefaultValueTransient_ClusterTester),
                nameof(TestResources.DefaultValueTransient_Cluster),
                TestResources.DefaultValueTransient_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
