using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Conditional.RContext.Hierarchy2
{

    [TestClass]
    public class ConditionalRContextHierarchy2Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalRContextHierarchy2Module.ConditionalRContextHierarchy2ModuleTester),
                nameof(TestResources.ConditionalRContextHierarchy2Module),
                TestResources.ConditionalRContextHierarchy2Module
                );

            preparation.Check();
        }

    }
}
