using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Conditional.Hierarchy3Second
{

    [TestClass]
    public class ConditionalHierarchy3SecondFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalHierarchy3SecondModule.ConditionalHierarchy3SecondModuleTester),
                nameof(TestResources.ConditionalHierarchy3SecondModule),
                TestResources.ConditionalHierarchy3SecondModule
                );

            preparation.Check();
        }

    }
}
