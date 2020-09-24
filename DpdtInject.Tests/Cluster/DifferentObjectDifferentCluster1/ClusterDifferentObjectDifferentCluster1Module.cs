using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster1
{
    public partial class ClusterDifferentObjectDifferentCluster1Module : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<ChildCluster>()
                ;
        }

        public partial class DefaultCluster //: DpdtCluster
        {
        }

        public partial class ChildCluster : DefaultCluster
        {
        }


        public class ClusterDifferentObjectDifferentCluster1ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ClusterDifferentObjectDifferentCluster1Module>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

                var b0 = module.Get<ChildCluster, IB>();
                Assert.IsNotNull(b0);

                Assert.AreSame(a0, b0.A);
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
        IA A
        {
            get;
        }
    }

    public class B : IB
    {
        public IA A
        {
            get;
        }

        public B(IA a)
        {
            A = a;
        }
    }

}
