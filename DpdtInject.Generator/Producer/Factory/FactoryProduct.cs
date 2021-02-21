using Microsoft.CodeAnalysis;
using System;

namespace DpdtInject.Generator.Producer.Factory
{
    internal class FactoryProduct
    {
        public ITypeSymbol FactoryType
        {
            get;
        }

        public string SourceCode
        {
            get;
        }

        public FactoryProduct(
            ITypeSymbol factoryType,
            string sourceCode
            )
        {
            if (factoryType is null)
            {
                throw new ArgumentNullException(nameof(factoryType));
            }

            if (sourceCode is null)
            {
                throw new ArgumentNullException(nameof(sourceCode));
            }

            FactoryType = factoryType;
            SourceCode = sourceCode;
        }
    }

}
