using DpdtInject.Injector.Module;

namespace DpdtInject.Tests.Performance.Generic.Transient
{
    public partial class DpdtPerformanceModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;

            Bind<IC>()
                .To<C>()
                .WithTransientScope()
                ;

        }
    }

}
