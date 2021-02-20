using System;
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

        public string GetBody()
        {
            return $@"if(parentCluster is null || !parentCluster.{nameof(ICluster.IsRegisteredFromRecursive)}<{ResolutionType.ToDisplayString()}>())
{{
    RaiseNoBindingAvailable<{ResolutionType.ToDisplayString()}>();
}}
";
        }
    }
}
