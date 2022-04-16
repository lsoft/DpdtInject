using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.AsyncDispose
{
    [TestClass]
    public class SingletonAsyncDispose_Fixture
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
                nameof(SingletonAsyncDispose_Cluster.SingletonAsyncDispose_ClusterTester),
                nameof(TestResources.SingletonAsyncDispose_Cluster),
                TestResources.SingletonAsyncDispose_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
