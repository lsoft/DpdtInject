using DpdtInject.Injector;
using DpdtInject.Injector.Module.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Conditional.NonGeneric.OnlyOneInMiddle
{
    public partial class ConditionalNonGenericOnlyOneInMiddle_Cluster : DefaultCluster
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
                .When(rc => false)
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .When(rc => true)
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                ;
        }

        public class ConditionalNonGenericOnlyOneInMiddle_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConditionalNonGenericOnlyOneInMiddle_Cluster>(
                    null
                    );

                var a0 = (IA)cluster.Get(typeof(IA));
                Assert.IsNotNull(a0);

                var b0 = (IB)cluster.Get(typeof(IB));
                Assert.IsNotNull(b0);
                Assert.AreSame(a0, b0.A);

                var c0 = (IC)cluster.Get(typeof(IC));
                Assert.IsNotNull(c0);
                Assert.AreSame(b0, c0.B);
                Assert.AreSame(a0, c0.B.A);
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
        IA A { get; }
    }

    public class B : IB
    {
        public IA A { get; }

        public B(IA a)
        {
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
