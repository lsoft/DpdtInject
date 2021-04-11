using DpdtInject.Generator.Core.Producer;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Excp;

namespace DpdtInject.Tests.Settings.CrossCluster.MustBeCrossCluster0
{
    [TestClass]
    public class SettingsCrossClusterMustBeCrossCluster0_Fixture
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
                nameof(SettingsCrossClusterMustBeCrossCluster0_ClusterTester),
                nameof(TestResources.SettingsCrossClusterMustBeCrossCluster0_Cluster),
                TestResources.SettingsCrossClusterMustBeCrossCluster0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(DpdtExceptionTypeEnum.LocalBindingFound, preparation.DiagnosticReporter.GetDpdtException().Type);
            Assert.AreEqual(typeof(IA).ToGlobalDisplayString(), preparation.DiagnosticReporter.GetDpdtException().AdditionalArgument);
        }
    }
}
