using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.GetAll.Generic.NoBinding
{
    public partial class GetAllGenericNoBinding_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            //nothing!
        }

        public class GetAllGenericNoBinding_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllGenericNoBinding_Cluster>(
                    null
                    );
                try
                {
                    var a0 = cluster.GetAll<IA>();
                    Assert.Fail("this line should never be executed");
                }
                catch (DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IA).FullName)
                {
                    //this is ok
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
}
