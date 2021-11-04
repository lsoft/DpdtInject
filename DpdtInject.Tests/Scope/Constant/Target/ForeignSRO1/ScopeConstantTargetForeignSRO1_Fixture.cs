using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.Target.ForeignSRO1
{
    [TestClass]
    public class ScopeConstantTargetForeignSRO1_Fixture
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
                nameof(ScopeConstantTargetForeignSRO1_Cluster.ScopeConstantTargetForeignSRO1_ClusterTester),
                nameof(TestResources.ScopeConstantTargetForeignSRO1_Cluster),
                TestResources.ScopeConstantTargetForeignSRO1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
