using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Factory.ConstructorArgument0
{
    public partial class FactoryConstructorArgument0_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IAFactory>()
                .ToIsolatedFactory<AFactory>()
                .WithPayload<A>()
                .WithSingletonScope()
                ;
        }

        public class FactoryConstructorArgument0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<FactoryConstructorArgument0_Cluster>(
                    null
                    );

                var af = cluster.Get<IAFactory>();
                Assert.IsNotNull(af);

                var a0 = af.Create("0");
                Assert.IsNotNull(a0);

                var a1 = af.Create("1");
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
        public A(string a)
        {

        }
    }

    public interface IAFactory
    {
        IA Create(string a);
    }

    public partial class AFactory : IFakeFactory<IAFactory>
    {
    }

}
