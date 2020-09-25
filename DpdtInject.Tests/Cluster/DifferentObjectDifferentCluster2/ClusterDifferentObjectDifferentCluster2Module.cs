using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster2
{
    public partial class ClusterDifferentObjectDifferentCluster2Module : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<ChildCluster2>()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<ChildCluster1>()
                ;
        }

        public partial class DefaultCluster 
        {
        }

        public partial class ChildCluster1 : DefaultCluster
        {
        }

        public partial class ChildCluster2 : DefaultCluster
        {
        }

        public class ClusterDifferentObjectDifferentCluster2ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ClusterDifferentObjectDifferentCluster2Module>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

                var a1 = module.Get<ChildCluster2, IA>();
                Assert.IsNotNull(a1);
                Assert.AreNotSame(a0, a1);

                var b0 = module.Get<ChildCluster1, IB>();
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
