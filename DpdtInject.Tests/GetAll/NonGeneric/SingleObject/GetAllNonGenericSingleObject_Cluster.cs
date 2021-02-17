using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DpdtInject.Tests.GetAll.NonGeneric.SingleObject
{
    public partial class GetAllNonGenericSingleObject_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class GetAllNonGenericSingleObject_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllNonGenericSingleObject_Cluster>(
                    null
                    );

                var alist = (List<IA>)cluster.GetAll(typeof(IA));
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
