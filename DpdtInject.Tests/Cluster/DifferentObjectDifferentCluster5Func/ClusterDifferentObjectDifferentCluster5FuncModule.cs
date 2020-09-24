using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster5Func
{
    public partial class ClusterDifferentObjectDifferentCluster5FuncModule : DpdtModule
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
                .InCluster<ChildCluster1>()
                ;
        }

        public partial class DefaultCluster
        {
        }

        public partial class ChildCluster1 : DefaultCluster
        {
        }

        public class ClusterDifferentObjectDifferentCluster5FuncModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ClusterDifferentObjectDifferentCluster5FuncModule>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

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

        public B(Func<IA> af)
        {
            A = af();
        }
    }

}
