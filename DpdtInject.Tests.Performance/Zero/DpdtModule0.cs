using DpdtInject.Injector.Module;

namespace DpdtInject.Tests.Performance.Zero
{
    public partial class DpdtModule0 : DpdtModule
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
