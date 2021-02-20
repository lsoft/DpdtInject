using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Unsorted.TwoBindMethods
{
    public partial class UnsortedTwoBindMethods_Cluster : DefaultCluster
    {
        public const string DefaultMessage = "default message";

        [DpdtBindingMethod]
        public void BindMethod1()
        {
            Bind<string>()
                .WithConstScope(DefaultMessage)
                ;
        }

        [DpdtBindingMethod]
        public void BindMethod2()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedTwoBindMethods_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedTwoBindMethods_Cluster>(
                    null
                    );

                var a = cluster.Get<IA>();
                Assert.IsNotNull(a);
                Assert.AreEqual(DefaultMessage, a.Message);
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
