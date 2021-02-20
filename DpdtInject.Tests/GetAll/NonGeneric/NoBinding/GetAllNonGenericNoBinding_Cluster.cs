using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DpdtInject.Tests.GetAll.NonGeneric.NoBinding
{
    public partial class GetAllNonGenericNoBinding_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            //nothing!
        }

        public class GetAllNonGenericNoBinding_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllNonGenericNoBinding_Cluster>(
                    null
                    );
                try
                {
                    var a0 = (List<IA>) cluster.GetAll(typeof(IA));

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
