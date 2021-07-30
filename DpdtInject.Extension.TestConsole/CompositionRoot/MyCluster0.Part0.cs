using DpdtInject.Extension.TestConsole.Payload;
using DpdtInject.Injector.Src;

namespace DpdtInject.Extension.TestConsole.CompositionRoot
{
    public partial class MyCluster0 : DefaultCluster
    {
        private readonly int[] AInts = new int[0];

        /// <inheritdoc />
        public MyCluster0(
            )
        {
        }


        [DpdtBindingMethod]
        public void BindMethod0()
        {
            Bind<int[]>()
                .WithConstScope(AInts)
                ;

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
