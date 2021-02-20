using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.NonGeneric.IncorrectRequest
{
    [TestClass]
    public class NonGenericIncorrectRequest_Fixture
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
                nameof(NonGenericIncorrectRequest_Cluster.NonGenericIncorrectRequest_ClusterTester),
                nameof(TestResources.NonGenericIncorrectRequest_Cluster),
                TestResources.NonGenericIncorrectRequest_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
