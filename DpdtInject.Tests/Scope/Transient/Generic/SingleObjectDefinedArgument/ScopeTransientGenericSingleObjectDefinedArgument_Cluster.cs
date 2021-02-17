using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Bind;

namespace DpdtInject.Tests.Scope.Transient.Generic.SingleObjectDefinedArgument
{
    public partial class ScopeTransientGenericSingleObjectDefinedArgument_Cluster : DefaultCluster
    {
        const string CustomMesage = "custom message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .Configure(new ConstructorArgument("message", CustomMesage))
                ;

        }

        public class ScopeTransientGenericSingleObjectDefinedArgument_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeTransientGenericSingleObjectDefinedArgument_Cluster>(
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
        string Message { get; }
    }

    public class A : IA
    {
        public string Message { get; }

        public A(string message)
        {
            Message = message;
        }

    }
}
