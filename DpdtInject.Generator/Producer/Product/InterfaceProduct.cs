using Microsoft.CodeAnalysis;
using System;

namespace DpdtInject.Generator.Producer.Product
{
    public interface IInterfaceProduct
    {
        string GetInterfaceDeclaration(ShortTypeNameGenerator sng);
    }

    public class NamedGeneric1Interface : IInterfaceProduct
    {
        public string InterfaceName
        {
            get;
        }
        public ITypeSymbol TypeSymbol
        {
            get;
        }

        public NamedGeneric1Interface(
            string interfaceName,
            ITypeSymbol typeSymbol
            )
        {
            if (interfaceName is null)
            {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            if (typeSymbol is null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            InterfaceName = interfaceName;
            TypeSymbol = typeSymbol;
        }


        public string GetInterfaceDeclaration(ShortTypeNameGenerator sng)
        {
            return $"{InterfaceName}<{sng.GetShortName(TypeSymbol)}>";
        }
    }
}
