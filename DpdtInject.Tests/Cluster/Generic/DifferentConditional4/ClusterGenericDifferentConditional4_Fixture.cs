using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.Generic.DifferentConditional4
{
    [TestClass]
    public class ClusterGenericDifferentConditional4_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterGenericDifferentConditional4_ClusterTester),
                nameof(TestResources.ClusterGenericDifferentConditional4_Cluster),
                TestResources.ClusterGenericDifferentConditional4_Cluster
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(DpdtExceptionTypeEnum.NoBindingAvailable, preparation.DiagnosticReporter.GetInnerDpdtException().Type);
            Assert.AreEqual(typeof(IB).FullName, preparation.DiagnosticReporter.GetInnerDpdtException().AdditionalArgument);
        }

    }
}
