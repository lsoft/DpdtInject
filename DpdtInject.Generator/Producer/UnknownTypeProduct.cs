using System;
using System.CodeDom.Compiler;
using DpdtInject.Injector;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer
{
    public class UnknownTypeProduct
    {
        public ITypeSymbol ResolutionType
        {
            get;
        }

        public UnknownTypeProduct(
            ITypeSymbol resolutionType
            )
        {
            if (resolutionType is null)
            {
                throw new ArgumentNullException(nameof(resolutionType));
            }

            ResolutionType = resolutionType;
        }

        internal void WriteBody(IndentedTextWriter2 writer)
        {
            writer.WriteLine2($@"if(parentCluster is null || !parentCluster.{nameof(ICluster.IsRegisteredFromRecursive)}<{ResolutionType.ToDisplayString()}>())
{{
    RaiseNoBindingAvailable<{ResolutionType.ToDisplayString()}>();
}}
");
        }
    }
}
