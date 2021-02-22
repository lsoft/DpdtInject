using System;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.ClassProducer
{
    internal class ProducedClassProduct
    {
        public ITypeSymbol FactoryType
        {
            get;
        }

        public string SourceCode
        {
            get;
        }

        public ProducedClassProduct(
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
