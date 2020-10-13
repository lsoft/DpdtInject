using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Module;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Scanner
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
