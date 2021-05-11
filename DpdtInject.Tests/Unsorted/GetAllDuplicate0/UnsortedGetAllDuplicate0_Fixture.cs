using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.GetAllDuplicate0
{
    [TestClass]
    public class UnsortedGetAllDuplicate0_Fixture
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
                nameof(UnsortedGetAllDuplicate0_Cluster.UnsortedGetAllDuplicate0_ClusterTester),
                nameof(TestResources.UnsortedGetAllDuplicate0_Cluster),
                TestResources.UnsortedGetAllDuplicate0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
