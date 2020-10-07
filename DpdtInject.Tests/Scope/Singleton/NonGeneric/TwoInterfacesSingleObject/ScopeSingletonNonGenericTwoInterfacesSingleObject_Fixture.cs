using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Singleton.NonGeneric.TwoInterfacesSingleObject
{
    [TestClass]
    public class ScopeSingletonNonGenericTwoInterfacesSingleObject_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeSingletonNonGenericTwoInterfacesSingleObject_Cluster.ScopeSingletonNonGenericTwoInterfacesSingleObject_ClusterTester),
                nameof(TestResources.ScopeSingletonNonGenericTwoInterfacesSingleObject_Cluster),
                TestResources.ScopeSingletonNonGenericTwoInterfacesSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
