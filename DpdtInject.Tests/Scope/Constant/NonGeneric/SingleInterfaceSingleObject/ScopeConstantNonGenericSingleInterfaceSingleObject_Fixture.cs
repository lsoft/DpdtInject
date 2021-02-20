using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.NonGeneric.SingleInterfaceSingleObject
{
    [TestClass]
    public class ScopeConstantNonGenericSingleInterfaceSingleObject_Fixture
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
                nameof(ScopeConstantNonGenericSingleInterfaceSingleObject_Cluster.ScopeConstantNonGenericSingleInterfaceSingleObject_ClusterTester),
                nameof(TestResources.ScopeConstantNonGenericSingleInterfaceSingleObject_Cluster),
                TestResources.ScopeConstantNonGenericSingleInterfaceSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
