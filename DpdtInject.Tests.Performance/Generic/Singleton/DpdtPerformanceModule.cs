using DpdtInject.Injector.Module;

namespace DpdtInject.Tests.Performance.Generic.Singleton
{
    public partial class DpdtPerformanceModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;

        }
        public partial class DefaultCluster
        {

        }
    }

}
