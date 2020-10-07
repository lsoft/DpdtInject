using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.RContext;

namespace DpdtInject.Tests.Scope.Custom.NonGeneric.Hierarchy2
{
    public partial class ScopeCustomNonGenericHierarchy2_Cluster : DefaultCluster
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithCustomScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .Configure(new ConstructorArgument("message", Message))
                ;
        }

        public class ScopeCustomNonGenericHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeCustomNonGenericHierarchy2_Cluster>(
                    null
                    );

                var scope0 = cluster.CreateCustomScope();
                var scope1 = cluster.CreateCustomScope();

                var b0_0 = (IB)cluster.Get(typeof(IB), scope0);
                Assert.IsNotNull(b0_0);
                Assert.IsNotNull(b0_0.A);
                Assert.AreEqual(Message, b0_0.Message);

                var b1 = (IB)cluster.Get(typeof(IB), scope1);
                Assert.IsNotNull(b1);
                Assert.IsNotNull(b1.A);
                Assert.AreEqual(Message, b1.Message);

                var b0_1 = (IB)cluster.Get(typeof(IB), scope0);
                Assert.IsNotNull(b0_1);
                Assert.IsNotNull(b0_1.A);
                Assert.AreEqual(Message, b0_1.Message);



                Assert.AreNotSame(b0_0, b0_1);
                Assert.AreSame(b0_0.A, b0_1.A);

                Assert.AreNotSame(b0_0, b1);
                Assert.AreNotSame(b0_0.A, b1.A);
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
        string Message { get; }

        IA A { get; }
    }

    public class B : IB
    {
        public string Message { get; }
        public IA A { get; }

        public B(string message, IA a)
        {
            Message = message;
            A = a;
        }
    }

}
