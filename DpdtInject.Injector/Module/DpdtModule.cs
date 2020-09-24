using DpdtInject.Injector.Module.Bind;
using System;
using System.Collections.Generic;

namespace DpdtInject.Injector.Module
{
    //public abstract class DpdtCluster
    //{

    //}

    public abstract class DpdtModule : IDisposable
    {
        public DpdtModule()
        {
        }

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

        public virtual void Dispose()
        {
            //nothing to do
        }

    }

}
