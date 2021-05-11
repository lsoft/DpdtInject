using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.GetAllDuplicate1
{
    [TestClass]
    public class UnsortedGetAllDuplicate1_Fixture
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
                nameof(UnsortedGetAllDuplicate1_Cluster.UnsortedGetAllDuplicate1_ClusterTester),
                nameof(TestResources.UnsortedGetAllDuplicate1_Cluster),
                TestResources.UnsortedGetAllDuplicate1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
