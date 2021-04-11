using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Conditional.Generic.Hierarchy3Second
{
    public partial class ConditionalGenericHierarchy3Second_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .When(rc => true)
                .Configure(new ConstructorArgument("message", Message))
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                ;
        }

        public class ConditionalGenericHierarchy3Second_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConditionalGenericHierarchy3Second_Cluster>(
                    null
                    );

                var c0 = cluster.Get<IC>();
                Assert.IsNotNull(c0);
                Assert.IsNotNull(c0.B);
                Assert.AreEqual(Message, c0.B.Message);
                Assert.IsNotNull(c0.B.A);

                var cc = cluster.GetAll<IC>();
                Assert.IsNotNull(cc);
                Assert.AreEqual(1, cc.Count);
                Assert.AreSame(c0, cc[0]);
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

    public interface IC
    {
        IB B
        {
            get;
        }
    }

    public class C : IC
    {
        public IB B
        {
            get;
        }

        public C(
            IB b
            )
        {
            B = b;
        }
    }
}
