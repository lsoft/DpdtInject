using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conventions.FromSingleInterface1
{
    [TestClass]
    public class ConventionsFromSingleInterface1_Fixture
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
                nameof(ConventionsFromSingleInterface1_Cluster.ConventionsFromSingleInterface1_ClusterTester),
                nameof(TestResources.ConventionsFromSingleInterface1_Cluster),
                TestResources.ConventionsFromSingleInterface1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
