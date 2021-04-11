using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Scope.Custom.Generic.Hierarchy2
{
    public partial class ScopeCustomGenericHierarchy2_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithCustomScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .Configure(new ConstructorArgument("message", Message))
                ;
        }

        public class ScopeCustomGenericHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeCustomGenericHierarchy2_Cluster>(
                    null
                    );

                var scope0 = cluster.CreateCustomScope();
                var scope1 = cluster.CreateCustomScope();

                var b0_0 = cluster.Get<IB>(scope0);
                Assert.IsNotNull(b0_0);
                Assert.IsNotNull(b0_0.A);
                Assert.AreEqual(Message, b0_0.Message);

                var b1 = cluster.Get<IB>(scope1);
                Assert.IsNotNull(b1);
                Assert.IsNotNull(b1.A);
                Assert.AreEqual(Message, b1.Message);

                var b0_1 = cluster.Get<IB>(scope0);
                Assert.IsNotNull(b0_1);
                Assert.IsNotNull(b0_1.A);
                Assert.AreEqual(Message, b0_1.Message);

                Assert.AreNotSame(b0_0, b0_1);
                Assert.AreSame(b0_0.A, b0_1.A);

                Assert.AreNotSame(b0_0, b1);
                Assert.AreNotSame(b0_0.A, b1.A);
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
        string Message
        {
            get;
        }

        IA A
        {
            get;
        }
    }

    public class B : IB
    {
        public string Message
        {
            get;
        }

        public IA A
        {
            get;
        }

        public B(
            string message,
            IA a
            )
        {
            Message = message;
            A = a;
        }
    }
}
