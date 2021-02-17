using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Binding
{
    public class BindingContainerGroup
    {
        private readonly List<BindingContainerExtender> _bindingExtenders;

        public ITypeSymbol BindFrom
        {
            get;
        }

        public IReadOnlyList<BindingContainerExtender> BindingExtenders => _bindingExtenders;

        public BindingContainerGroup(
            ITypeSymbol bindFrom
            )
        {
            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            BindFrom = bindFrom;
            _bindingExtenders = new List<BindingContainerExtender>();
        }

        public void Add(BindingContainerExtender bindingExtender)
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
