using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Wrapper.Func.Singleton
{
    public partial class WrapperFuncSingleton_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;

        }

        public class WrapperFuncSingleton_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<WrapperFuncSingleton_Cluster>(
                    null
                    );

                var a0 = cluster.Get<IA>();
                Assert.IsNotNull(a0);

                var af1 = cluster.Get<Func<IA>>();
                Assert.IsNotNull(af1);
                var a1 = af1();

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
}
