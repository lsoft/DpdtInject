using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Scope.Singleton.Generic.Hierarchy2
{
    public partial class ScopeSingletonGenericHierarchy2_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                ;
        }

        public class ScopeSingletonGenericHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeSingletonGenericHierarchy2_Cluster>(
                    null
                    );

                var a = cluster.Get<IA>();
                Assert.IsNotNull(a);

                var b0 = cluster.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);

                Assert.AreSame(a, b0.A);

                var b1 = cluster.Get<IB>();
                Assert.IsNotNull(b1);
                Assert.IsNotNull(b1.A);
                Assert.AreSame(b0, b1);

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
        IA A { get; }
    }

    public class B : IB
    {
        public IA A { get; }

        public B(IA a)
        {
            A = a;
        }

    }
}
