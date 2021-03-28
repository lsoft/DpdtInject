using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conventions.ToSingleInterface0
{
    [TestClass]
    public class ConventionsToSingleInterface0_Fixture
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
                nameof(ConventionsToSingleInterface0_Cluster.ConventionsToSingleInterface0_ClusterTester),
                nameof(TestResources.ConventionsToSingleInterface0_Cluster),
                TestResources.ConventionsToSingleInterface0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
