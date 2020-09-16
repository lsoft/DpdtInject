using DpdtInject.Injector;
using DpdtInject.Injector.Module;

namespace DpdtInject.Binding.Example.Zero
{
    public partial class DpdtModule0 : DpdtModule
    {
        public const string Message = "some message";

        public override void Load()
        {
            //Bind<IA>()
            //    .To<A>()
            //    .WithSingletonScope()
            //    ;

            //Bind<IB>()
            //    .To<B>()
            //    .WithSingletonScope()
            //    .Configure(new ConstructorArgument("message", Message))
            //    ;

            //Bind<IC>()
            //    .To<C>()
            //    .WithSingletonScope()
            //    ;

        }

    }

}

