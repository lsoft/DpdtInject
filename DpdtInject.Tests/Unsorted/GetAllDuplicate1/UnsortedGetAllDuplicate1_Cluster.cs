using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Tests.Unsorted.GetAllDuplicate1
{
    public partial class UnsortedGetAllDuplicate1_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<HandlerContainer>()
                .To<HandlerContainer>()
                .WithTransientScope()
#if IN_UNIT_TEST_SYMBOL
                .Configure(new ConstructorArgument("handlers", this.GetAll<IHandler>()))
#endif
                ;

            ScanInAssembliesWith<Handler1>()
                .SelectAllWith<IHandler>()
                .FromAllInterfaces()
                .ToItself()
                .WithTransientScope()
                //.When(rt => true)
                ;
        }

        public class UnsortedGetAllDuplicate1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedGetAllDuplicate1_Cluster>(
                    null
                    );

                var hc = cluster.Get<HandlerContainer>();
                Assert.IsNotNull(hc);
                Assert.AreEqual(2, hc.Handlers.Count());
            }
        }
    }


    public class HandlerContainer
    {
        public IEnumerable<IHandler> Handlers
        {
            get;
        }

        public HandlerContainer(
            IEnumerable<IHandler> handlers
            )
        {
            Handlers = handlers;
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
