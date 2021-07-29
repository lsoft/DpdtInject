using DpdtInject.Injector.Src.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.DynamicConstructorArgument0
{
    [TestClass]
    public class UnsortedDynamicConstructorArgument0_Fixture
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
                nameof(UnsortedDynamicConstructorArgument0_Cluster.UnsortedDynamicConstructorArgument0_ClusterTester),
                nameof(TestResources.UnsortedDynamicConstructorArgument0_Cluster),
                TestResources.UnsortedDynamicConstructorArgument0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
