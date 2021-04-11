using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Excp;

namespace DpdtInject.Tests.Proxy.TwoSameProxy
{
    [TestClass]
    public class ProxyTwoSameProxy_Fixture
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
                nameof(ProxyTwoSameProxy_Cluster.ProxyTwoSameProxy_ClusterTester),
                nameof(TestResources.ProxyTwoSameProxy_Cluster),
                TestResources.ProxyTwoSameProxy_Cluster
                );

            preparation.Check();

            //check for only ony proxy class was generated; in this case we will receive DpdtExceptionTypeEnum.DuplicateBinding exception
            //otherwise, we will receive strange compilation error and AssertFailException
            Assert.AreEqual(1, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(DpdtExceptionTypeEnum.DuplicateBinding, preparation.DiagnosticReporter.GetInnerDpdtException().Type);
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
