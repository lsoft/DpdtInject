using DpdtInject.Injector;

namespace DpdtInject.Tests.Unsorted.CircularDependency3
{
    public partial class UnsortedCircularDependency3_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;

            Bind<IC>()
                .To<C>()
                .WithTransientScope()
                ;
        }

        public class UnsortedCircularDependency3Cluster_Tester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedCircularDependency3_Cluster>(
                    null
                    );
            }
        }

    }


    public interface IA
    {
        IC C { get; }
    }

    public class A : IA
    {
        public IC C { get; }

        public A(IC c)
        {
            C = c;
        }
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
