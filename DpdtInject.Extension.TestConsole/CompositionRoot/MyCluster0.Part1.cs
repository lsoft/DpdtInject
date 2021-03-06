using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Extension.TestConsole.Payload;
using DpdtInject.Injector;

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
        }
    }

}
