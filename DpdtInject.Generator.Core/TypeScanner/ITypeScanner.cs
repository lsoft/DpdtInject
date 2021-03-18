using System.Collections.Generic;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.TypeScanner
{
    public interface ITypeScanner
    {
        IReadOnlyList<INamedTypeSymbol> ScanForClusterTypes(
            ITypeInfoProvider typeInfoProvider
            );
    }
}
