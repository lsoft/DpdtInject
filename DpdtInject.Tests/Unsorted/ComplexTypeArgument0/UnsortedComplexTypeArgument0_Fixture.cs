using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.ComplexTypeArgument0
{
    [TestClass]
    public class UnsortedComplexTypeArgument0_Fixture
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
                nameof(UnsortedComplexTypeArgument0_Cluster.UnsortedComplexTypeArgument0_ClusterTester),
                nameof(TestResources.UnsortedComplexTypeArgument0_Cluster),
                TestResources.UnsortedComplexTypeArgument0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
