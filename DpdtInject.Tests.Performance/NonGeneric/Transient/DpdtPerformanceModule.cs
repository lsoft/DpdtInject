using DpdtInject.Injector.Module;

namespace DpdtInject.Tests.Performance.NonGeneric.Transient
{
    public partial class DpdtPerformanceModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IC>()
                .To<C>()
                .WithTransientScope()
                .InCluster<DefaultCluster>()
                ;

        }

        public partial class DefaultCluster
        {

        }
    }

}
