using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Excp;

namespace DpdtInject.Tests.Unsorted.CannotBeCasted
{
    [TestClass]
    public class UnsortedCannotBeCasted_Fixture
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
                nameof(UnsortedCannotBeCasted_Cluster.UnsortedCannotBeCasted_ClusterTester),
                nameof(TestResources.UnsortedCannotBeCasted_Cluster),
                TestResources.UnsortedCannotBeCasted_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(1, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
            Assert.AreEqual(DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom, preparation.DiagnosticReporter.GetDpdtException().Type);
        }
    }
}
