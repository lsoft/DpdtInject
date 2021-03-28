using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.Target.InPlace0
{
    [TestClass]
    public class ScopeConstantTargetInPlace0_Fixture
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
                nameof(ScopeConstantTargetInPlace0_Cluster.ScopeConstantTargetInPlace0_ClusterTester),
                nameof(TestResources.ScopeConstantTargetInPlace0_Cluster),
                TestResources.ScopeConstantTargetInPlace0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
