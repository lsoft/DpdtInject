using DpdtInject.Generator.BindExtractor;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
