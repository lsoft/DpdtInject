using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Excp;

namespace DpdtInject.Tests.Unsorted.InvalidToType
{
    [TestClass]
    public class UnsortedInvalidToType_Fixture
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
                nameof(UnsortedInvalidToType_Cluster.UnsortedInvalidToType_ClusterTester),
                nameof(TestResources.UnsortedInvalidToType_Cluster),
                TestResources.UnsortedInvalidToType_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(1, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
            Assert.AreEqual(DpdtExceptionTypeEnum.IncorrectBinding_IncorrectTarget, preparation.DiagnosticReporter.GetDpdtException().Type);
        }
    }
}
