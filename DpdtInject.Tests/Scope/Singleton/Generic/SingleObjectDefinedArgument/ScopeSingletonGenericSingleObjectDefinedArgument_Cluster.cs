using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Bind;

namespace DpdtInject.Tests.Scope.Singleton.Generic.SingleObjectDefinedArgument
{
    public partial class ScopeSingletonGenericSingleObjectDefinedArgument_Cluster : DefaultCluster
    {
        const string CustomMesage = "custom message";

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("message", CustomMesage))
                ;
        }

        public class ScopeSingletonGenericSingleObjectDefinedArgument_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeSingletonGenericSingleObjectDefinedArgument_Cluster>(
                    null
                    );

                var a0 = cluster.Get<IA>();
                Assert.IsNotNull(a0);
                Assert.AreEqual(CustomMesage, a0.Message);
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
