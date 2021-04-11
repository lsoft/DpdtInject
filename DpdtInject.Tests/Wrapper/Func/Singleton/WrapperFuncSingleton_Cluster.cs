using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src.Bind.Settings.Wrapper;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Wrapper.Func.Singleton
{
    public partial class WrapperFuncSingleton_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .Setup<ProduceWrappers>()
                ;
        }

        public class WrapperFuncSingleton_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<WrapperFuncSingleton_Cluster>(
                    null
                    );

                var a0 = cluster.Get<IA>();
                Assert.IsNotNull(a0);

                var af1 = cluster.Get<Func<IA>>();
                Assert.IsNotNull(af1);
                var a1 = af1();

                Assert.AreSame(a0, a1);
            }
        }
    }


    public interface IA
    {
    }

    public class A : IA
    {
    }
}
