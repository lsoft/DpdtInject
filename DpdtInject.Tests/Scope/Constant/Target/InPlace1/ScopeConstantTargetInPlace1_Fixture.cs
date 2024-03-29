using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.Target.InPlace1
{
    [TestClass]
    public class ScopeConstantTargetInPlace1_Fixture
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
                nameof(ScopeConstantTargetInPlace1_Cluster.ScopeConstantTargetInPlace1_ClusterTester),
                nameof(TestResources.ScopeConstantTargetInPlace1_Cluster),
                TestResources.ScopeConstantTargetInPlace1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
