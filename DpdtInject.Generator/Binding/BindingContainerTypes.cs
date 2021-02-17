using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Binding
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

        public ITypeSymbol? FactoryPayloadType
        {
            get;
        }

        public bool ToFactory => !(FactoryPayloadType is null);

        public BindingContainerTypes(
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol bindToType,
            ITypeSymbol? factoryPayloadType
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
            FactoryPayloadType = factoryPayloadType;
        }

    }
}