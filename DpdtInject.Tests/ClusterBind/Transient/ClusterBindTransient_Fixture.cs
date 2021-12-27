using DpdtInject.Injector.Src.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.ClusterBind.Transient
{
    [TestClass]
    public class ClusterBindTransient_Fixture
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
                nameof(ClusterBindTransient_Cluster.ClusterBindTransient_ClusterTester),
                nameof(TestResources.ClusterBindTransient_Cluster),
                TestResources.ClusterBindTransient_Cluster
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(DpdtExceptionTypeEnum.BindToClusterType, preparation.DiagnosticReporter.GetDpdtException().Type);
            Assert.AreEqual(typeof(SubjectCluster).FullName, preparation.DiagnosticReporter.GetDpdtException().AdditionalArgument);
        }
    }
}
