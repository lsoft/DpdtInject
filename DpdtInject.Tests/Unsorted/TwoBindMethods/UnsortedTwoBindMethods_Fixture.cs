using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.TwoBindMethods
{
    [TestClass]
    public class UnsortedTwoBindMethods_Fixture
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
                nameof(UnsortedTwoBindMethods_Cluster.UnsortedTwoBindMethods_ClusterTester),
                nameof(TestResources.UnsortedTwoBindMethods_Cluster),
                TestResources.UnsortedTwoBindMethods_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
