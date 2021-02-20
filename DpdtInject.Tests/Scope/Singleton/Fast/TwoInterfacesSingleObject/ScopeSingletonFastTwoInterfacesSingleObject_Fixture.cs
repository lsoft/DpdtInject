using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.Fast.TwoInterfacesSingleObject
{
    [TestClass]
    public class ScopeSingletonFastTwoInterfacesSingleObject_Fixture
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
                nameof(ScopeSingletonFastTwoInterfacesSingleObject_Cluster.ScopeSingletonFastTwoInterfacesSingleObject_ClusterTester),
                nameof(TestResources.ScopeSingletonFastTwoInterfacesSingleObject_Cluster),
                TestResources.ScopeSingletonFastTwoInterfacesSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
