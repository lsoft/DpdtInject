using System.Collections.Generic;
using DpdtInject.Generator.TypeInfo;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.TypeScanner
{
    public interface ITypeScanner
    {
        IReadOnlyList<INamedTypeSymbol> ScanForClusterTypes(
            ITypeInfoProvider typeInfoProvider
            );
    }
}
