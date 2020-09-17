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

        //public virtual void Apply(
        //    IInstancesContainer containers
        //    )
        //{
        //    throw new InvalidOperationException($"dpdt is a source-generation based DI container. This method should not be performed. Probably something went wrong with your {GetType().Name} module.");
        //}


        protected IToBinding Bind<T1, T2>(
            //string name = BindingNameTree.RootName
            )
        {
            //if (name is null)
            //{
            //    throw new ArgumentNullException(nameof(name));
            //}

            var bind = new DefineBindingNode(
                //name,
                new List<Type>
                {
                    typeof(T1),
                    typeof(T2)
                });

            _binds.Add(bind);

            return bind;
        }

        protected IToBinding Bind<T>(
            //string name = BindingNameTree.RootName
            )
        {
            //if (name is null)
            //{
            //    throw new ArgumentNullException(nameof(name));
            //}

            var bind = new DefineBindingNode(
                //name,
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
