using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Unsorted.InvalidToType
{
    public partial class UnsortedInvalidToType_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        public override void Load()
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
