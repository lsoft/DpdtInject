using DpdtInject.Injector;

namespace DpdtInject.Tests.Performance.Generic.Transient
{
    public partial class DpdtCluster : DefaultCluster
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
