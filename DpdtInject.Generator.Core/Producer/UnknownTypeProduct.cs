using System;
using DpdtInject.Injector;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Producer
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

        internal void WriteBody(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.WriteLine2($@"if(parentCluster is null || !parentCluster.{nameof(ICluster.IsRegisteredFromRecursive)}<{sng.GetShortName(ResolutionType)}>())
{{
    RaiseNoBindingAvailable<{sng.GetShortName(ResolutionType)}>();
}}
");
        }
    }
}
