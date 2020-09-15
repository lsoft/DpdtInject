using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Injector.Module.Bind
{
    public class BindNode
    {
        //public string Name { get; }

        public IReadOnlyList<Type> BindsFrom { get; }

        public string BindsFromFullName => string.Join(",", BindsFrom.Select(b => b.FullName));

        public string BindsFromName => string.Join(",", BindsFrom.Select(b => b.Name));

        public Type BindTo { get; private set; }

        public BindScopeEnum BindScope { get; private set; }

        public bool IsCompleted => BindTo != null;

        public BindNode(
            IReadOnlyList<Type> bindsFrom,
            Type bindTo,
            BindScopeEnum bindScope
            //string name = ""
            )
        {
            if (bindsFrom is null)
            {
                throw new ArgumentNullException(nameof(bindsFrom));
            }

            if (bindTo is null)
            {
                throw new ArgumentNullException(nameof(bindTo));
            }

            //if (name is null)
            //{
            //    throw new ArgumentNullException(nameof(name));
            //}

            BindsFrom = bindsFrom;
            BindTo = bindTo;
            BindScope = bindScope;
            //Name = name;
        }

    }
}
