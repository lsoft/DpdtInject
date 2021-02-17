using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Bind;

namespace DpdtInject.Tests.Scope.Custom.NonGeneric.Hierarchy2DifferentCluster
{
    public partial class ScopeCustomNonGenericHierarchy2DifferentCluster_FirstCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithCustomScope()
                ;
        }
    }

    public partial class ScopeCustomNonGenericHierarchy2DifferentCluster_LastCluster : DefaultCluster
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .Configure(new ConstructorArgument("message", Message))
                ;
        }
    }

    public class ScopeCustomNonGenericHierarchy2DifferentCluster_ClusterTester
    {
        public void PerformClusterTesting()
        {
            var firstCluster = new FakeCluster<ScopeCustomNonGenericHierarchy2DifferentCluster_FirstCluster>(
                null
                );
            var lastCluster = new FakeCluster<ScopeCustomNonGenericHierarchy2DifferentCluster_LastCluster>(
                firstCluster
                );

            var scope0 = lastCluster.CreateCustomScope();
            var scope1 = lastCluster.CreateCustomScope();

            var b0_0 = (IB)lastCluster.Get(typeof(IB), scope0);
            Assert.IsNotNull(b0_0);
            Assert.IsNotNull(b0_0.A);
            Assert.AreEqual(ScopeCustomNonGenericHierarchy2DifferentCluster_LastCluster.Message, b0_0.Message);

            var b1 = (IB)lastCluster.Get(typeof(IB), scope1);
            Assert.IsNotNull(b1);
            Assert.IsNotNull(b1.A);
            Assert.AreEqual(ScopeCustomNonGenericHierarchy2DifferentCluster_LastCluster.Message, b1.Message);

            var b0_1 = (IB)lastCluster.Get(typeof(IB), scope0);
            Assert.IsNotNull(b0_1);
            Assert.IsNotNull(b0_1.A);
            Assert.AreEqual(ScopeCustomNonGenericHierarchy2DifferentCluster_LastCluster.Message, b0_1.Message);



            Assert.AreNotSame(b0_0, b0_1);
            Assert.AreSame(b0_0.A, b0_1.A);

            Assert.AreNotSame(b0_0, b1);
            Assert.AreNotSame(b0_0.A, b1.A);
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
