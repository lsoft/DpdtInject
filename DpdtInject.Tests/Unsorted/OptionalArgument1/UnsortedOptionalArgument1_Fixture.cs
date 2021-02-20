using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.OptionalArgument1
{
    [TestClass]
    public class UnsortedOptionalArgument1_Fixture
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
                nameof(UnsortedOptionalArgument1_Cluster.UnsortedOptionalArgument1_ClusterTester),
                nameof(TestResources.UnsortedOptionalArgument1_Cluster),
                TestResources.UnsortedOptionalArgument1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
