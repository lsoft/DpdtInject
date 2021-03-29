using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conventions.FromSingleInterface0
{
    [TestClass]
    public class ConventionsFromSingleInterface0_Fixture
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
                nameof(ConventionsFromSingleInterface0_Cluster.ConventionsFromSingleInterface0_ClusterTester),
                nameof(TestResources.ConventionsFromSingleInterface0_Cluster),
                TestResources.ConventionsFromSingleInterface0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
