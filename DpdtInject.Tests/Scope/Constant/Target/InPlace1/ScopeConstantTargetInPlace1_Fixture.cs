using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;


//using DpdtInject.Injector.Reinvented;
//using Type_79c652ca = System.Type;
//using Func_IResolutionRequest__object__84a55a07 = System.Func<DpdtInject.Injector.RContext.IResolutionRequest, object>;
//using System;

namespace DpdtInject.Tests.Scope.Constant.Target.InPlace1
{
    [TestClass]
    public class ScopeConstantTargetInPlace1_Fixture
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
                nameof(ScopeConstantTargetInPlace1_Cluster.ScopeConstantTargetInPlace1_ClusterTester),
                nameof(TestResources.ScopeConstantTargetInPlace1_Cluster),
                TestResources.ScopeConstantTargetInPlace1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");


            //var _typeContainerGet = new FixedSizeFactoryContainer(
            //    new System.Tuple<Type_79c652ca, Func_IResolutionRequest__object__84a55a07>(typeof(ulong), (r) => Get_ulong_4aad6cd5dd294f25a65e4e04962e70a9(r))
            //    );

        }

        //private string Get_ulong_4aad6cd5dd294f25a65e4e04962e70a9(DpdtInject.Injector.RContext.IResolutionRequest resolutionRequest)
        //{
        //    return default;
        //}
    }
}
