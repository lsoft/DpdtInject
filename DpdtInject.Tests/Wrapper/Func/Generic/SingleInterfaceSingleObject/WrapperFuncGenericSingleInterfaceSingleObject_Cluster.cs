using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Wrapper.Func.Generic.SingleInterfaceSingleObject
{
    public partial class WrapperFuncGenericSingleInterfaceSingleObject_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

        }

        public class WrapperFuncGenericSingleInterfaceSingleObject_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<WrapperFuncGenericSingleInterfaceSingleObject_Cluster>(
                    null
                    );

                var a0 = cluster.Get<IA>();
                Assert.IsNotNull(a0);

                var af1 = cluster.Get<Func<IA>>();
                Assert.IsNotNull(af1);
                var a1 = af1();

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
}
