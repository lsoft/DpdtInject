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
                .When(rc => true)
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .When(rc => true)
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                .When(rc => true)
                ;

        }
    }

}
