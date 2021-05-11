using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Unsorted.GetAllDuplicate0
{
    public partial class UnsortedGetAllDuplicate0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            ScanInAssembliesWith<Handler1>()
                .SelectAllWith<IHandler>()
                .FromAllInterfaces()
                .ToItself()
                .WithTransientScope()
                .When(rt => true)
                ;
        }

        public class UnsortedGetAllDuplicate0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedGetAllDuplicate0_Cluster>(
                    null
                    );

                var handlers = cluster.GetAll<IHandler>();
                Assert.IsNotNull(handlers);
                Assert.AreEqual(2, handlers.Count);
            }
        }
    }


    public interface IHandler
    {
    
    }

    public class Handler1 : IHandler
    {

    }

    public class Handler2 : IHandler
    {
    
    }
}
