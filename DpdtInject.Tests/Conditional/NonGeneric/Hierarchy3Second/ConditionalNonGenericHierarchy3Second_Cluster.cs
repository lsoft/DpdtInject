using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DpdtInject.Injector.Bind;

namespace DpdtInject.Tests.Conditional.NonGeneric.Hierarchy3Second
{
    public partial class ConditionalNonGenericHierarchy3Second_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        public override void Load()
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

        public class ConditionalNonGenericHierarchy3Second_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConditionalNonGenericHierarchy3Second_Cluster>(
                    null
                    );

                var c0 = (IC)cluster.Get(typeof(IC));
                Assert.IsNotNull(c0);
                Assert.IsNotNull(c0.B);
                Assert.AreEqual(Message, c0.B.Message);
                Assert.IsNotNull(c0.B.A);

                var cc = (List<IC>)cluster.GetAll(typeof(IC));
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
        string Message { get; }

        IA A { get; }
    }

    public class B : IB
    {
        public string Message { get; }
        public IA A { get; }

        public B(string message, IA a)
        {
            Message = message;
            A = a;
        }
    }

    public interface IC
    {
        IB B { get; }
    }

    public class C : IC
    {
        public IB B { get; }

        public C(IB b)
        {
            B = b;
        }
    }


}
