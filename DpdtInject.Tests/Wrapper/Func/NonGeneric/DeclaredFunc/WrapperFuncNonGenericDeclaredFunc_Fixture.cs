using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Wrapper.Func.NonGeneric.DeclaredFunc
{
    [TestClass]
    public class WrapperFuncNonGenericDeclaredFunc_Fixture
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
                nameof(WrapperFuncNonGenericDeclaredFunc_Cluster.WrapperFuncNonGenericDeclaredFunc_ClusterTester),
                nameof(TestResources.WrapperFuncNonGenericDeclaredFunc_Cluster),
                TestResources.WrapperFuncNonGenericDeclaredFunc_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
