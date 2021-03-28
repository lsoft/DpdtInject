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
        public const string BindMethodName = nameof(Bind);
        public const string ScanInAssembliesWithMethodName = nameof(ScanInAssembliesWith);

        protected IConventionalBinding ScanInAssembliesWith<T1>(
            )
        {
            throw new NotImplementedException();
        }

        protected IConventionalBinding ScanInAssembliesWith<T1, T2>(
            )
        {
            throw new NotImplementedException();
        }

        protected IConventionalBinding ScanInAssembliesWith<T1, T2, T3>(
            )
        {
            throw new NotImplementedException();
        }





        protected IToOrConstantBinding Bind<T1, T2, T3, T4, T5, T6>(
            )
        {
            throw new NotImplementedException();
        }

        protected IToOrConstantBinding Bind<T1, T2, T3, T4, T5>(
            )
        {
            throw new NotImplementedException();
        }

        protected IToOrConstantBinding Bind<T1, T2, T3, T4>(
            )
        {
            throw new NotImplementedException();
        }

        protected IToOrConstantBinding Bind<T1, T2, T3>(
            )
        {
            throw new NotImplementedException();
        }

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
