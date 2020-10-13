using DpdtInject.Generator.TypeInfo;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DpdtInject.Generator.Scanner
{
    public interface ITypeScanner
    {
        IReadOnlyList<INamedTypeSymbol> Scan(
            ITypeInfoProvider typeInfoProvider
            );
    }
}
