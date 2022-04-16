using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Custom.AsyncDispose
{
    [TestClass]
    public class CustomAsyncDispose_Fixture
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
                nameof(CustomAsyncDispose_Cluster.CustomAsyncDispose_ClusterTester),
                nameof(TestResources.CustomAsyncDispose_Cluster),
                TestResources.CustomAsyncDispose_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
