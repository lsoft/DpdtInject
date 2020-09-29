using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.CustomScope;
using System;
using System.Collections.Generic;

namespace DpdtInject.Injector.Module
{
    public interface ICustomScopeFactory
    {
        CustomScopeObject CreateCustomScope();
    }


    public abstract class DpdtModule : IDisposable
    {
        public DpdtModule()
        {
        }

        protected CustomScopeObject CreateCustomScope(int estimatedTypeCount)
        {
            return new CustomScopeObject(estimatedTypeCount);
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
