using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster3
{
    public partial class ClusterTwoDifferentObjectDifferentCluster3Module : DpdtModule
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

        public class ClusterTwoDifferentObjectDifferentCluster3ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ClusterTwoDifferentObjectDifferentCluster3Module>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

                var b0 = module.Get<ChildCluster1, IB>();
                Assert.IsNotNull(b0);
                Assert.AreSame(a0, b0.A);

                var b1 = module.Get<ChildCluster2, IB>();
                Assert.IsNotNull(b1);
                Assert.AreNotSame(b0, b1);
                Assert.AreSame(b0.A, b1.A);
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
