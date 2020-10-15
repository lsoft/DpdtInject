using DpdtInject.Injector.CustomScope;
using DpdtInject.Injector.Module.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector
{
    public abstract class DefaultCluster
    {
        public abstract void Load();

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
