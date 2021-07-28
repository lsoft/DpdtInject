using DpdtInject.Injector.Src.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.Dynamic1
{
    [TestClass]
    public class UnsortedDynamic1_Fixture
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
                nameof(UnsortedDynamic1_Cluster.UnsortedDynamic1_ClusterTester),
                nameof(TestResources.UnsortedDynamic1_Cluster),
                TestResources.UnsortedDynamic1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(1, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
            Assert.AreEqual(DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom, preparation.DiagnosticReporter.GetDpdtException().Type);
        }
    }
}
