using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Wrapper.Func.Generic.SingleInterfaceSingleObject
{
    [TestClass]
    public class WrapperFuncGenericSingleInterfaceSingleObject_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(WrapperFuncGenericSingleInterfaceSingleObject_Cluster.WrapperFuncGenericSingleInterfaceSingleObject_ClusterTester),
                nameof(TestResources.WrapperFuncGenericSingleInterfaceSingleObject_Cluster),
                TestResources.WrapperFuncGenericSingleInterfaceSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
