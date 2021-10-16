using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Core.Binding
{
    public class BindingExtenderGroup
    {
        private readonly List<BindingExtender> _bindingExtenders;

        public ITypeSymbol BindFrom
        {
            get;
        }

        public IReadOnlyList<BindingExtender> BindingExtenders => _bindingExtenders;

        public BindingExtenderGroup(
            ITypeSymbol bindFrom
            )
        {
            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            BindFrom = bindFrom;
            _bindingExtenders = new List<BindingExtender>();
        }

        public void Add(BindingExtender bindingExtender)
        {
            if (bindingExtender is null)
            {
                throw new ArgumentNullException(nameof(bindingExtender));
            }

            _bindingExtenders.Add(
                bindingExtender
                );
        }

    }
}
