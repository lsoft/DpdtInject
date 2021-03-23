using DpdtInject.Extension.TestConsole.Payload;
using DpdtInject.Injector;
using DpdtInject.Extension.TestConsole.Payload.NewNamespace1;
using DpdtInject.Extension.TestConsole.Payload.NewNamespace1.NewNamespace2;
using System;
using DpdtInject.Injector.Bind;
namespace DpdtInject.Extension.TestConsole.CompositionRoot
{
    public partial class MyCluster0 : DefaultCluster
    {
        /// <inheritdoc />
        public MyCluster0(
            )
        {
        }


        [DpdtBindingMethod]
        public void BindMethod0()
        {
            Bind<BindPayload0>()
                .To<BindPayload0>()
                .WithSingletonScope()
                ;

            Bind<BindPayload1>()
                .To<BindPayload1>()
                .WithSingletonScope()
                ;

            Bind<BindPayload1>()
                .To<BindPayload1>()
                .WithTransientScope()
                .When(rt => true)
                ;
        }
    }

}
