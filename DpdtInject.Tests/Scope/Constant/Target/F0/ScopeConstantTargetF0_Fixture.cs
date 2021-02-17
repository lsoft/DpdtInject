using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.Target.F0
{
    [TestClass] 
    public class ScopeConstantTargetF0_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeConstantTargetF0_Cluster.ScopeConstantTargetF0_ClusterTester),
                nameof(TestResources.ScopeConstantTargetF0_Cluster),
                TestResources.ScopeConstantTargetF0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(1, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }

    }
}
