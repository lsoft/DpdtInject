using DpdtInject.Injector.Module;

namespace DpdtInject.Tests.Performance.Conditional.Singleton
{
    public partial class DpdtPerformanceModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => true)
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => true)
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => true)
                ;

        }

        public partial class DefaultCluster
        { 
        }

    }

}
