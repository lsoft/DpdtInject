using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Transient.Generic.SingleObjectDefinedArgument
{
    [TestClass]
    public class ScopeTransientGenericSingleObjectDefinedArgument_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeTransientGenericSingleObjectDefinedArgument_Cluster.ScopeTransientGenericSingleObjectDefinedArgument_ClusterTester),
                nameof(TestResources.ScopeTransientGenericSingleObjectDefinedArgument_Cluster),
                TestResources.ScopeTransientGenericSingleObjectDefinedArgument_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
