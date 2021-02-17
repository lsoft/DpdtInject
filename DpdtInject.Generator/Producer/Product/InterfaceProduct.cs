using System;

namespace DpdtInject.Generator.Producer.Product
{
    public class InterfaceProduct
    {
        public string InterfaceDeclaration
        {
            get;
        }

        public InterfaceProduct(
            string interfaceDeclaration
            )
        {
            if (interfaceDeclaration is null)
            {
                throw new ArgumentNullException(nameof(interfaceDeclaration));
            }

            InterfaceDeclaration = interfaceDeclaration;
        }

    }
}
