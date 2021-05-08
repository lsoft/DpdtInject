using DpdtInject.Extension.TestConsole.Payload;
using DpdtInject.Injector.Src;

namespace DpdtInject.Extension.TestConsole.CompositionRoot
{
    public partial class MyCluster0 : DefaultCluster
    {

        [DpdtBindingMethod]
        public void BindMethod1()
        {
            Bind<BindPayload1>()
                .To<BindPayload1>()
                .WithCustomScope()
                ;

            ScanInAssembliesWith<IConventional>()
                .SelectAllWith<IConventional>()
                .FromAllInterfaces()
                .ToItself()
                .WithSingletonScope()
                ;
        }
    }

}
