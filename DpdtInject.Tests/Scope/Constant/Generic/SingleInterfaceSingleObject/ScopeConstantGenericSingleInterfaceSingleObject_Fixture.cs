using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.Generic.SingleInterfaceSingleObject
{
    [TestClass]
    public class ScopeConstantGenericSingleInterfaceSingleObject_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeConstantGenericSingleInterfaceSingleObject_Cluster.ScopeConstantGenericSingleInterfaceSingleObject_ClusterTester),
                nameof(TestResources.ScopeConstantGenericSingleInterfaceSingleObject_Cluster),
                TestResources.ScopeConstantGenericSingleInterfaceSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
