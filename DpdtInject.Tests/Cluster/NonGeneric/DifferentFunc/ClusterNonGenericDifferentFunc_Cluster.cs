using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Cluster.NonGeneric.DifferentFunc
{
    public partial class ClusterNonGenericDifferentFunc_RootCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;
        }
    }

    public partial class ClusterNonGenericDifferentFunc_ChildCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }
    }

    public class ClusterNonGenericDifferentFunc_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterNonGenericDifferentFunc_RootCluster>(
                null
                );
            var childCluster = new FakeCluster<ClusterNonGenericDifferentFunc_ChildCluster>(
                rootCluster
                );

            var a = (IA)rootCluster.Get(typeof(IA));
            Assert.IsNotNull(a);

            var b0 = (IB)childCluster.Get(typeof(IB));
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
