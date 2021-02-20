using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.Generic.Hierarchy2
{
    public partial class ConditionalGenericHierarchy2_Cluster : DefaultCluster
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

        public class ConditionalGenericHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConditionalGenericHierarchy2_Cluster>(
                    null
                    );

                var a = cluster.Get<IA>();
                Assert.IsNotNull(a);
                Assert.IsTrue(a.GetType() == typeof(A1));

                var b0 = cluster.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);
                Assert.AreNotSame(a, b0.A);
                Assert.IsTrue(b0.A.GetType() == typeof(A1));

                var b1 = cluster.Get<IB>();
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
