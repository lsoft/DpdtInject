using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Wrapper.Func.Generic.DeclaredFunc
{
    public partial class WrapperFuncGenericDeclaredFunc_Cluster : DefaultCluster
    {
        public static readonly A AInstance = new();
        public static readonly Func<IA> Funca = () => AInstance;

        public override void Load()
        {

            Bind<Func<IA>>()
                .WithConstScope(Funca)
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }

        public class WrapperFuncGenericDeclaredFunc_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<WrapperFuncGenericDeclaredFunc_Cluster>(
                    null
                    );

                var b0 = cluster.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);
                Assert.AreSame(AInstance, b0.A);

                var aff0 = cluster.Get<Func<Func<IA>>>();
                Assert.IsNotNull(aff0);
                var af0 = aff0();
                Assert.AreSame(Funca, af0);
                var a0 = af0();
                Assert.AreSame(AInstance, a0);

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
        IA A { get; }
    }

    public class B : IB
    {
        public IA A { get; }

        public B(Func<IA> af)
        {
            A = af();
        }

    }

}
