using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.TypeScanner
{
    internal class DefaultTypeScanner : ITypeScanner

    {
        public IReadOnlyList<INamedTypeSymbol> Scan(
            ITypeInfoProvider typeInfoProvider
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            var allTypes = typeInfoProvider.GetAllTypes().ToList();

            var foundTypes = allTypes
                .Where(t => t.BaseType != null)
                .Where(t => t.BaseType!.ToDisplayString() == typeof(DefaultCluster).FullName)
                .ToList()
                ;

            return foundTypes;
        }
    }
}
