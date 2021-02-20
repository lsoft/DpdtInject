using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Wrapper.Func.NonGeneric.SingleInterfaceSingleObject
{
    public partial class WrapperFuncNonGenericSingleInterfaceSingleObject_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;
        }

        public class WrapperFuncNonGenericSingleInterfaceSingleObject_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<WrapperFuncNonGenericSingleInterfaceSingleObject_Cluster>(
                    null
                    );

                var a0 = (IA) cluster.Get(typeof(IA));
                Assert.IsNotNull(a0);

                var af1 = (Func<IA>) cluster.Get(typeof(Func<IA>));
                Assert.IsNotNull(af1);
                var a1 = af1();

                Assert.AreNotSame(a0, a1);
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
