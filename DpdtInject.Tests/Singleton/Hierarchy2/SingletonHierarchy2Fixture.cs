using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Singleton.Hierarchy2
{

    [TestClass]
    public class SingletonHierarchy2Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(SingletonHierarchy2Module.SingletonHierarchy2ModuleTester),
                nameof(TestResources.SingletonHierarchy2Module),
                TestResources.SingletonHierarchy2Module
                );

            preparation.Check();
        }

    }
}
