using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.NonGeneric.Hierarchy2
{
    public partial class ConditionalNonGenericHierarchy2_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A0>()
                .WithTransientScope()
                .When(rc => false)
                ;

            Bind<IA>()
                .To<A1>()
                .WithTransientScope()
                .When(rc => true)
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }

        public class ConditionalNonGenericHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConditionalNonGenericHierarchy2_Cluster>(
                    null
                    );

                var a = (IA) cluster.Get(typeof(IA));
                Assert.IsNotNull(a);
                Assert.IsTrue(a.GetType() == typeof(A1));

                var b0 = (IB) cluster.Get(typeof(IB));
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);
                Assert.AreNotSame(a, b0.A);
                Assert.IsTrue(b0.A.GetType() == typeof(A1));

                var b1 = (IB) cluster.Get(typeof(IB));
                Assert.IsNotNull(b1);
                Assert.IsNotNull(b1.A);
                Assert.AreNotSame(b0, b1);
                Assert.IsTrue(b1.A.GetType() == typeof(A1));
            }
        }
    }


    public interface IA
    {
    }

    public class A0 : IA
    {
    }

    public class A1 : IA
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
            IA a
            )
        {
            A = a;
        }
    }
}
