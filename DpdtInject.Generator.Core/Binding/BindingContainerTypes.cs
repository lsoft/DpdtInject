using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Binding
{
    public class BindingContainerTypes
    {
        public IReadOnlyList<ITypeSymbol> BindFromTypes
        {
            get;
        }

        public ITypeSymbol BindToType
        {
            get;
        }


        public BindingContainerTypes(
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol bindToType
            )
        {
            if (bindFromTypes is null)
            {
                throw new System.ArgumentNullException(nameof(bindFromTypes));
            }

            if (bindToType is null)
            {
                throw new System.ArgumentNullException(nameof(bindToType));
            }

            BindFromTypes = bindFromTypes;
            BindToType = bindToType;
        }

    }
}