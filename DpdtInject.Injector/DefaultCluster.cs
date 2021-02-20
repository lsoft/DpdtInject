using DpdtInject.Injector.CustomScope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Injector.Bind;

namespace DpdtInject.Injector
{
    public abstract class DefaultCluster
    {
        protected IToOrConstantBinding Bind<T1, T2>(
            )
        {
            throw new NotImplementedException();
        }

        protected IToOrConstantBinding Bind<T>(
            )
        {
            throw new NotImplementedException();
        }
    }
}
