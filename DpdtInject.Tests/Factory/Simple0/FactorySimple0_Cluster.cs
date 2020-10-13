using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Factory.Simple0
{
    public partial class FactorySimple0_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IAFactory>()
                .ToFactory<AFactory>()
                .WithSingletonScope()
                ;
        }

        public class FactorySimple0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<FactorySimple0_Cluster>(
                    null
                    );

                var a0 = cluster.Get<IA>();
                Assert.IsNotNull(a0);

                var a1 = cluster.Get<IA>();
                Assert.IsNotNull(a1);

                Assert.AreSame(a0, a1);
            }
        }

    }


    public interface IA
    {

    }

    public class A : IA
    {

    }

    public interface IAFactory
    {
        IA Create();
    }

    public partial class AFactory
    {
    }

}
