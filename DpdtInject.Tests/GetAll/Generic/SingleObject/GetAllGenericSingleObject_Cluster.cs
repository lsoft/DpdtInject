using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.GetAll.Generic.SingleObject
{
    public partial class GetAllGenericSingleObject_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class GetAllGenericSingleObject_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllGenericSingleObject_Cluster>(
                    null
                    );

                var alist = cluster.GetAll<IA>();
                Assert.IsNotNull(alist);
                Assert.AreEqual(1, alist.Count);
                Assert.IsNotNull(alist[0]);
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
