using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.Generic.Hierarchy2SameConstant
{
    [TestClass]
    public class ConditionalGenericHierarchy2SameConstant_Fixture
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
                nameof(ConditionalGenericHierarchy2SameConstant_Cluster.ConditionalGenericHierarchy2SameConstant_ClusterTester),
                nameof(TestResources.ConditionalGenericHierarchy2SameConstant_Cluster),
                TestResources.ConditionalGenericHierarchy2SameConstant_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
