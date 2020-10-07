using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.GetAll.Generic.TwoObjectsConditional1
{
    public partial class GetAllGenericTwoObjectsConditional1_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A0>()
                .WithSingletonScope()
                .When(rt => rt.IsGetAllResolution)
                ;

            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                ;
        }

        public class GetAllGenericTwoObjectsConditional1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllGenericTwoObjectsConditional1_Cluster>(
                    null
                    );

                var alist = cluster.GetAll<IA>();
                Assert.IsNotNull(alist);
                Assert.AreEqual(2, alist.Count);
                Assert.IsTrue(alist.Any(a => a.ImplementationType == typeof(A0)));
                Assert.IsTrue(alist.Any(a => a.ImplementationType == typeof(A1)));
            }
        }

    }


    public interface IA
    {
        Type ImplementationType
        {
            get;
        }
    }

    public class A0 : IA
    {
        public Type ImplementationType => typeof(A0);
    }

    public class A1 : IA
    {
        public Type ImplementationType => typeof(A1);
    }
}
