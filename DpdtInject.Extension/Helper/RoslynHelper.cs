using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //var btLevel = level;
            //var baseType = s.BaseType;
            //while (baseType != null)
            //{
            //    yield return (btLevel, baseType);

            //    baseType = baseType.BaseType;
            //    btLevel++;
            //}

            //yield return (level, s);
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
    }
}
