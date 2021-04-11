using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Unsorted.InvalidToType
{
    public partial class UnsortedInvalidToType_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<IA>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("message", Message))
                ;
        }

        public class UnsortedInvalidToType_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedInvalidToType_Cluster>(
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

    public class A : IA
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
