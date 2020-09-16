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
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                ;

        }
    }

}
