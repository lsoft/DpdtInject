using DpdtInject.Generator.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Generator
{
    public class TypeSymbolEqualityComparer : IEqualityComparer<ITypeSymbol>
    {
        public bool Equals([AllowNull] ITypeSymbol x, [AllowNull] ITypeSymbol y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null)
            {
                return false;
            }
            if (y is null)
            {
                return false;
            }

            if (x.ToDisplayString() == y.ToDisplayString())
            {
                return true;
            }

            return false;
        }

        public int GetHashCode([DisallowNull] ITypeSymbol obj)
        {
            return obj.ToDisplayString().GetHashCode();
        }
    }
}
