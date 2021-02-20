using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.Generic.Hierarchy2SameConstant
{
    public partial class ConditionalGenericHierarchy2SameConstant_Cluster : DefaultCluster
    {
        public static readonly A ConstantA1 = new();
        public static readonly A ConstantA2 = new();

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .WithConstScope(ConstantA1)
                .When(rt => rt.ParentTarget.TargetType == typeof(B1))
                ;

            Bind<IA>()
                .WithConstScope(ConstantA2)
                .When(rt => rt.ParentTarget.TargetType == typeof(B2))
                ;

            Bind<IB1>()
                .To<B1>()
                .WithSingletonScope()
                ;

            Bind<IB2>()
                .To<B2>()
                .WithSingletonScope()
                ;
        }

        public class ConditionalGenericHierarchy2SameConstant_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConditionalGenericHierarchy2SameConstant_Cluster>(
                    null
                    );

                var b1 = cluster.Get<IB1>();
                Assert.IsNotNull(b1);
                Assert.AreSame(ConstantA1, b1.A);

                var b2 = cluster.Get<IB2>();
                Assert.IsNotNull(b2);
                Assert.AreSame(ConstantA2, b2.A);
            }
        }
    }


    public interface IA
    {
    }

    public class A : IA
    {
    }

    public interface IB1
    {
        IA A
        {
            get;
        }
    }

    public class B1 : IB1
    {
        public IA A
        {
            get;
        }

        public B1(
            IA a
            )
        {
            A = a;
        }
    }

    public interface IB2
    {
        IA A
        {
            get;
        }
    }

    public class B2 : IB2
    {
        public IA A
        {
            get;
        }

        public B2(
            IA a
            )
        {
            A = a;
        }
    }
}
