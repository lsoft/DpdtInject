using DpdtInject.Injector.Module;
using System.Linq;
using DpdtInject.Injector.Module.Bind;

namespace DpdtInject.Binding.Example.Conditional
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
            //    .When(rc => true)
            //    .Configure(new ConstructorArgument("message", Message))
            //    ;

            //Bind<IC>()
            //    .To<C>()
            //    .WithSingletonScope()
            //    ;
        }

    }
}
