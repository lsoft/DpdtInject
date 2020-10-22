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
            Bind<IAFactory>()
                .ToIsolatedFactory<AFactory>()
                .WithPayload<A>()
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

                var af = cluster.Get<IAFactory>();
                Assert.IsNotNull(af);

                var a0 = af.Create();
                Assert.IsNotNull(a0);

                var a1 = af.Create();
                Assert.IsNotNull(a1);

                Assert.AreNotSame(a0, a1);
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
        //string GetSomeString();

        IA Create();
    }

    public partial class AFactory : IFakeFactory<IAFactory>
    {
        //public string GetSomeString() => "1";

        //public IA Create() { throw new NotImplementedException(); }
    }

}
