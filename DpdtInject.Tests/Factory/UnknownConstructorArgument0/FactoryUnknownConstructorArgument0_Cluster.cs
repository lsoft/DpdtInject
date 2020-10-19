using DpdtInject.Injector;
using DpdtInject.Injector.Module.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Factory.UnknownConstructorArgument0
{
    public partial class FactoryUnknownConstructorArgument0_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IAFactory>()
                .ToFactory<AFactory, A>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("b", 1))
                ;
        }

        public class FactoryUnknownConstructorArgument0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<FactoryUnknownConstructorArgument0_Cluster>(
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
        public A(string a, int b)
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
