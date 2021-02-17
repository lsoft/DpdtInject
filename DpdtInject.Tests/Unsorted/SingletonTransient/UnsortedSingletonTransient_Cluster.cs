using DpdtInject.Injector;

namespace DpdtInject.Tests.Unsorted.SingletonTransient
{
    public partial class UnsortedSingletonTransient_Cluster : DefaultCluster
    {
        public const string DefaultMessage = "default message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedSingletonTransient_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedSingletonTransient_Cluster>(
                    null
                    );
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
}
