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

namespace DpdtInject.Tests.Scope.Custom.Hierarchy2ChildCluster
{
    public partial class ScopeCustomHierarchy2ChildClusterModule : DpdtModule
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithCustomScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .InCluster<ChildCluster>()
                .Configure(new ConstructorArgument("message", Message))
                ;
        }

        public partial class DefaultCluster
        {

        }

        public partial class ChildCluster : DefaultCluster
        {

        }

        public class ScopeCustomHierarchy2ChildClusterModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ScopeCustomHierarchy2ChildClusterModule>();

                var scope0 = module.CreateCustomScope();
                var scope1 = module.CreateCustomScope();

                var b0_0 = (IB)module.Get<ChildCluster>(typeof(IB), scope0);
                Assert.IsNotNull(b0_0);
                Assert.IsNotNull(b0_0.A);
                Assert.AreEqual(Message, b0_0.Message);

                var b1 = module.Get<ChildCluster, IB>(scope1);
                Assert.IsNotNull(b1);
                Assert.IsNotNull(b1.A);
                Assert.AreEqual(Message, b1.Message);

                var b0_1 = module.Get<ChildCluster, IB>(scope0);
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
