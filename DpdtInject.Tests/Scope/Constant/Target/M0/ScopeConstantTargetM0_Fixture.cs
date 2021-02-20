using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.Target.M0
{
    [TestClass]
    public class ScopeConstantTargetM0_Fixture
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
                nameof(ScopeConstantTargetM0_Cluster.ScopeConstantTargetM0_ClusterTester),
                nameof(TestResources.ScopeConstantTargetM0_Cluster),
                TestResources.ScopeConstantTargetM0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(1, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
