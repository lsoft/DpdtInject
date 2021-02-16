using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Unsorted.GenericInterface
{
    public partial class UnsortedGenericInterface_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<To<string>>()
                .To<ToIsolatedFactory<int>>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedGenericInterface_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedGenericInterface_Cluster>(
                    null
                    );

                var a = cluster.Get<To<string>>();
                Assert.IsNotNull(a);
            }
        }

    }


    /// <summary>
    /// Keep its name! this name should be equal "To"
    /// for an additional parser's test
    /// </summary>
    public abstract class To<T>
    {
    }

    /// <summary>
    /// Keep its name! this name should be equal "ToIsolatedFactory"
    /// for an additional parser's test
    /// </summary>
    public class ToIsolatedFactory<T> : To<string>
    {
    }

}
