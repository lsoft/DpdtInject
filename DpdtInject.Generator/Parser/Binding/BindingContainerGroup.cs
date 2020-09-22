using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingContainerGroup
    {
        private readonly List<IBindingContainer> _bindingContainers;

        public ITypeSymbol BindFrom
        {
            get;
        }

        public IReadOnlyList<IBindingContainer> BindingContainers => _bindingContainers;

        public BindingContainerGroup(
            ITypeSymbol bindFrom
            )
        {
            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            BindFrom = bindFrom;
            _bindingContainers = new List<IBindingContainer>();
        }

        public void Add(IBindingContainer bindingContainer)
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            _bindingContainers.Add(bindingContainer);
        }

    }
}
