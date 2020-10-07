using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
