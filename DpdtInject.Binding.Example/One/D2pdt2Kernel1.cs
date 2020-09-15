using DpdtInject.Injector;
using DpdtInject.Injector.Module;

namespace DpdtInject.Binding.Example.One
{
    //public partial class D2pdt2Kernel1 : D2pdt2Kernel
    //{ 

    //}

    public partial class D2pdt2Module1 : DpdtModule
    {
        public const string SomeString = "hello guys!";

        public override void Load()
        {
            //Bind<IA>()
            //    .To<A1>()
            //    .WithSingletonScope()
            //    .Configure(new ConstructorArgument("message", SomeString))
            //    ;

            //Bind<IA>()
            //    .To<A2>()
            //    .WithSingletonScope()
            //    //.Configure(new ConstructorArgument("message", SomeString))
            //    ;
        }

    }

}

