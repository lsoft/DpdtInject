using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Wrapper.Func.Generic.DeclaredFunc
{
    [TestClass]
    public class WrapperFuncGenericDeclaredFunc_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(WrapperFuncGenericDeclaredFunc_Cluster.WrapperFuncGenericDeclaredFunc_ClusterTester),
                nameof(TestResources.WrapperFuncGenericDeclaredFunc_Cluster),
                TestResources.WrapperFuncGenericDeclaredFunc_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
