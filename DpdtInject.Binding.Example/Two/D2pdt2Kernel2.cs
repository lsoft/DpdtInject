using DpdtInject.Injector;
using DpdtInject.Injector.Module;

namespace DpdtInject.Binding.Example.Two
{
    //public partial class D2pdt2Kernel2 : D2pdt2Kernel
    //{ 

    //}

    public partial class D2pdt2Module2 : DpdtModule
    {
        public override void Load()
        {
            //Bind<IA>()
            //    .To<A>()
            //    .WithSingletonScope()
            //    .Configure(new ConstructorArgument("message", SomeString))
            //    ;

            //Bind<IB>()
            //    .To<B>()
            //    .WithSingletonScope()
            //    ;

        }

    }

}

