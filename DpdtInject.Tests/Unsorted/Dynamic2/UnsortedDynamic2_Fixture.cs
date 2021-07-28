using DpdtInject.Injector.Src.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.Dynamic2
{
    [TestClass]
    public class UnsortedDynamic2_Fixture
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
                nameof(UnsortedDynamic2_Cluster.UnsortedDynamic2_ClusterTester),
                nameof(TestResources.UnsortedDynamic2_Cluster),
                TestResources.UnsortedDynamic2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(1, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
            Assert.AreEqual(DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom, preparation.DiagnosticReporter.GetDpdtException().Type);
        }
    }
}
