using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.RContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Scope.Custom.TwoSameBindings
{
    public partial class ScopeCustomTwoSameBindingsModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A1>()
                .WithCustomScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IA>()
                .To<A2>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class ScopeCustomTwoSameBindingsModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ScopeCustomTwoSameBindingsModule>();

                var aa = module.GetAll<IA>();
                Assert.IsNotNull(aa);
                Assert.AreEqual(1, aa.Count);

                var scope1 = new CustomScopeObject();

                var bb = module.GetAll<IA>(scope1);
                Assert.IsNotNull(bb);
                Assert.AreEqual(2, bb.Count);
                Assert.IsTrue(bb.Any(b => b.Type == nameof(A1)));
                Assert.IsTrue(bb.Any(b => b.Type == nameof(A2)));
            }
        }

    }


    public interface IA
    {
        string Type
        {
            get;
        }
    }

    public class A1 : IA
    {
        public string Type => nameof(A1);
    }

    public class A2 : IA
    {
        public string Type => nameof(A2);
    }
}
