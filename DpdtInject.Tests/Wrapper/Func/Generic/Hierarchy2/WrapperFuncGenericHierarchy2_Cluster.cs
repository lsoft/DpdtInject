using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src.Bind.Settings.Wrapper;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Wrapper.Func.Generic.Hierarchy2
{
    public partial class WrapperFuncGenericHierarchy2_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .Setup<ProduceWrappers>()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }

        public class WrapperFuncGenericHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<WrapperFuncGenericHierarchy2_Cluster>(
                    null
                    );

                var a = cluster.Get<IA>();
                Assert.IsNotNull(a);

                var b0 = cluster.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);
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

        public B(
            Func<IA> af
            )
        {
            A = af();
        }
    }
}
