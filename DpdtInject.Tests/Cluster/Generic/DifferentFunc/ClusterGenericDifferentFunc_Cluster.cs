using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Cluster.Generic.DifferentFunc
{
    public partial class ClusterGenericDifferentFunc_RootCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;
        }
    }

    public partial class ClusterGenericDifferentFunc_ChildCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }
    }

    public class ClusterGenericDifferentFunc_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterGenericDifferentFunc_RootCluster>(
                null
                );
            var childCluster = new FakeCluster<ClusterGenericDifferentFunc_ChildCluster>(
                rootCluster
                );

            var a = rootCluster.Get<IA>();
            Assert.IsNotNull(a);

            var b0 = childCluster.Get<IB>();
            Assert.IsNotNull(b0);
            Assert.IsNotNull(b0.A);
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

        public B(Func<IA> af)
        {
            A = af();
        }

    }
}
