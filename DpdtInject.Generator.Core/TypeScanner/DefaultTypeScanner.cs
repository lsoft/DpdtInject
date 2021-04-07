using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.TypeScanner
{
    internal class DefaultTypeScanner : ITypeScanner

    {
        public IReadOnlyList<INamedTypeSymbol> ScanForClusterTypes(
            ITypeInfoProvider typeInfoProvider
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            var foundTypes = typeInfoProvider.GetClusterTypes(RoslynHelper.IsClusterType).ToList();

            return foundTypes;
        }
    }
}
