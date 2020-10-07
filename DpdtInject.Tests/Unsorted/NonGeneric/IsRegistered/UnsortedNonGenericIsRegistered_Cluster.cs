using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Unsorted.NonGeneric.IsRegistered
{
    public partial class UnsortedNonGenericIsRegistered_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }

        public class UnsortedNonGenericIsRegistered_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedNonGenericIsRegistered_Cluster>(
                    null
                    );

                var a = cluster.IsRegisteredFrom(typeof(IA));
                Assert.IsTrue(a);

                var b = cluster.IsRegisteredFrom(typeof(IB));
                Assert.IsTrue(b);

                var c = cluster.IsRegisteredFrom(typeof(IC));
                Assert.IsFalse(c);
            }
        }

    }



    public interface IA
    {
    }

    public class A : IA
    {
    }

    public interface IB
    {
    }

    public class B : IB
    {
    }

    public interface IC
    {
    }
}
