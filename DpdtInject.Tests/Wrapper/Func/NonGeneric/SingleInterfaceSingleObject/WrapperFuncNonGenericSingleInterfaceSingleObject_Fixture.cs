using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Wrapper.Func.NonGeneric.SingleInterfaceSingleObject
{
    [TestClass]
    public class WrapperFuncNonGenericSingleInterfaceSingleObject_Fixture
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
                nameof(WrapperFuncNonGenericSingleInterfaceSingleObject_Cluster.WrapperFuncNonGenericSingleInterfaceSingleObject_ClusterTester),
                nameof(TestResources.WrapperFuncNonGenericSingleInterfaceSingleObject_Cluster),
                TestResources.WrapperFuncNonGenericSingleInterfaceSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
