using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Extension.Helper
{
    public static class RoslynHelper
    {
        public static IEnumerable<(int, INamedTypeSymbol)> IterateInterfaces(
            this INamedTypeSymbol s,
            int level = 0
            )
        {
            foreach (var i in s.Interfaces)
            {
                yield return (level, i);

                foreach (var child in IterateInterfaces(i, level + 1))
                {
                    yield return child;
                }
            }
        }

        public static IEnumerable<(int, INamedTypeSymbol)> IterateClasses(
            this INamedTypeSymbol s,
            int level = 0
            )
        {
            var baseType = s.BaseType;
            while (baseType != null)
            {
                yield return (level, baseType);

                baseType = baseType.BaseType;
                level++;
            }

            yield return (0, s);
        }

        public static IEnumerable<INamedTypeSymbol> GetAllTypes(this INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
                foreach (var nestedType in type.GetNestedTypes())
                    yield return nestedType;

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
                foreach (var type in nestedNamespace.GetAllTypes())
                    yield return type;
        }


        public static IEnumerable<INamedTypeSymbol> GetNestedTypes(this INamedTypeSymbol type)
        {
            yield return type;
            foreach (var nestedType in type.GetTypeMembers()
                .SelectMany(nestedType => nestedType.GetNestedTypes()))
                yield return nestedType;
        }

    }
}
