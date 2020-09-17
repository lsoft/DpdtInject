using DpdtInject.Injector.Module.Bind;
using System;
using System.Collections.Generic;

namespace DpdtInject.Injector.Module
{
    public abstract class DpdtModule : IDisposable
    {
        private readonly List<DefineBindingNode> _binds;

        public DpdtModule()
        {
            _binds = new List<DefineBindingNode>();
        }

        public abstract void Load();

        protected IToBinding Bind<T1, T2>(
            )
        {
            var bind = new DefineBindingNode(
                new List<Type>
                {
                    typeof(T1),
                    typeof(T2)
                });

            _binds.Add(bind);

            return bind;
        }

        protected IToBinding Bind<T>(
            )
        {
            var bind = new DefineBindingNode(
                new List<Type>
                {
                    typeof(T)
                });

            _binds.Add(bind);

            return bind;
        }

        public virtual void Dispose()
        {
            //nothing to do
        }

    }

}
