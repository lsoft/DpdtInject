using DpdtInject.Injector;
using DpdtInject.Injector.Module;

namespace DpdtInject.Binding.Example.Three
{
    //public partial class D2pdt2Kernel0 : D2pdt2Kernel
    //{ 

    //}

    public partial class D2pdt2Module3 : DpdtModule
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

        }

    }

}

