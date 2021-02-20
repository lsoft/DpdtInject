using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Wrapper.Func.Generic.Hierarchy2
{
    [TestClass]
    public class WrapperFuncGenericHierarchy2_Fixture
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
                nameof(WrapperFuncGenericHierarchy2_Cluster.WrapperFuncGenericHierarchy2_ClusterTester),
                nameof(TestResources.WrapperFuncGenericHierarchy2_Cluster),
                TestResources.WrapperFuncGenericHierarchy2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
