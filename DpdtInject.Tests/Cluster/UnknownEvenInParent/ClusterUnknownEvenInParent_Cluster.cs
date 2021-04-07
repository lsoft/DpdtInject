using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Bind.Settings.CrossCluster;

namespace DpdtInject.Tests.Cluster.UnknownEvenInParent
{
    public partial class ClusterUnknownEvenInParent_RootCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            //this cluster SHOULD NOT have a IA binding
            //because we're checked for UNKNOWN binding EVEN in parent cluster
        }
    }

    public partial class ClusterUnknownEvenInParent_ChildCluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .Setup<AllowedCrossCluster>()
                ;
        }
    }

    public class ClusterUnknownEvenInParent_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var rootCluster = new FakeCluster<ClusterUnknownEvenInParent_RootCluster>(
                null
                );

            try
            {
                var childCluster = new FakeCluster<ClusterUnknownEvenInParent_ChildCluster>(
                    rootCluster
                    );

                Assert.Fail("this line should never be executed");
            }
            catch (DpdtException excp)
                when (excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IA).FullName)
            {
                //it's OK, this test is green
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
        IA A
        {
            get;
        }
    }

    public class B : IB
    {
        public IA A
        {
            get;
        }

        public B(
            IA a
            )
        {
            A = a;
        }
    }
}
