using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Unsorted.CannotBeCasted
{
    public partial class UnsortedCannotBeCasted_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("message", Message))
                ;
        }

        public class UnsortedCannotBeCasted_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedCannotBeCasted_Cluster>(
                    null
                    );
            }
        }
    }


    public interface IA
    {
        string Message
        {
            get;
        }
    }

    public class A
    {
        public string Message
        {
            get;
        }

        public A(
            string message
            )
        {
            Message = message;
        }
    }
}
