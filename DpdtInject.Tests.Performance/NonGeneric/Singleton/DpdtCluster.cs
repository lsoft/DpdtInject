using DpdtInject.Injector;

namespace DpdtInject.Tests.Performance.NonGeneric.Singleton
{
    public partial class DpdtCluster : DefaultCluster
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
