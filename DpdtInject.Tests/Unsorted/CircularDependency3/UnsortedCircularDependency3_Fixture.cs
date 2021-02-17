using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.CircularDependency3
{

    [TestClass]
    public class UnsortedCircularDependency3_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedCircularDependency3_Cluster.UnsortedCircularDependency3Cluster_Tester),
                nameof(TestResources.UnsortedCircularDependency3_Cluster),
                TestResources.UnsortedCircularDependency3_Cluster
                );

            preparation.Check();

            //no check for warning because here is a lot of warnings due to circular dependency
            Assert.AreEqual(1, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
            Assert.AreEqual(DpdtExceptionTypeEnum.CircularDependency, preparation.DiagnosticReporter.GetDpdtException().Type);
        }

    }
}
