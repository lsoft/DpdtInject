using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.NonGeneric.IncorrectRequest
{
    public partial class NonGenericIncorrectRequest_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class NonGenericIncorrectRequest_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<NonGenericIncorrectRequest_Cluster>(
                    null
                    );

                try
                {
                    var r = cluster.Get(typeof(IB));

                    Assert.Fail("test fails, this line should never be executed");
                }
                catch (DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IB).FullName)
                {
                    //it's ok
                }

                try
                {
                    var rs = cluster.GetAll(typeof(IB)).ToList();

                    Assert.Fail("test fails, this line should never be executed");
                }
                catch (DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IB).FullName)
                {
                    //it's ok
                }
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
    }
}
