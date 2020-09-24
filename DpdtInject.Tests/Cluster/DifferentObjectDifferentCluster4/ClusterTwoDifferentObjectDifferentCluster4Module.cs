using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster4
{
    public partial class ClusterTwoDifferentObjectDifferentCluster4Module : DpdtModule
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
                .InCluster<ChildCluster1>()
                .When(rc => rc.IsRoot)
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<ChildCluster2>()
                ;
        }

        public partial class DefaultCluster
        {
            //public DefaultCluster()
            //{

            //}
        }

        public partial class ChildCluster1 : DefaultCluster
        {
        }

        public partial class ChildCluster2 : ChildCluster1
        {
        }

        public class ClusterTwoDifferentObjectDifferentCluster4ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ClusterTwoDifferentObjectDifferentCluster4Module>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

                var a1 = module.Get<ChildCluster1, IA>();
                Assert.IsNotNull(a1);
                Assert.AreNotSame(a0, a1);

                var b0 = module.Get<ChildCluster2, IB>();
                Assert.IsNotNull(b0);
                Assert.AreSame(a0, b0.A);
                Assert.AreNotSame(a1, b0.A);
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
