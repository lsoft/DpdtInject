using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DpdtInject.Tests.Scope.Constant.Target.InPlace2
{
    [TestClass]
    public class ScopeConstantTargetInPlace2_Fixture
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
                nameof(ScopeConstantTargetInPlace2_Cluster.ScopeConstantTargetInPlace2_ClusterTester),
                nameof(TestResources.ScopeConstantTargetInPlace2_Cluster),
                TestResources.ScopeConstantTargetInPlace2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");

        }
    }
}
