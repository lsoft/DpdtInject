using DpdtInject.Injector.Module;

namespace DpdtInject.Tests.Performance.Conditional.Transient
{
    public partial class DpdtPerformanceModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .When(rc => true)
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .When(rc => true)
                ;

            Bind<IC>()
                .To<C>()
                .WithTransientScope()
                .When(rc => true)
                ;

        }
    }

}
