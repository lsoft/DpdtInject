using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Cluster.Conditional1
{
    public partial class ClusterConditional1Module : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => !rc.IsRoot && rc.ParentFrame.ClusterDeclaredType == typeof(ChildCluster1))
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<ChildCluster1>()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<ChildCluster2>()
                ;
        }

        public partial class DefaultCluster
        {
        }

        public partial class ChildCluster1 : DefaultCluster
        {
        }

        public partial class ChildCluster2 : DefaultCluster
        {
        }

        public class ClusterConditional1Tester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ClusterConditional1Module>();

                try
                {
                    var a0 = module.Get<IA>();

                    Assert.Fail("this line should never be executed");
                }
                catch (DpdtException excp)
                    when(excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IA).FullName)
                {
                    //this is ok
                }

                var b0 = module.Get<ChildCluster1, IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);

                try
                {
                    var b1 = module.Get<ChildCluster2, IB>();

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

        public B(IA a)
        {
            A = a;
        }
    }

}
