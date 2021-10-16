using DpdtInject.Generator.Core.Producer;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Generator.Core
{
    /// <summary>
    /// Roslyn symbol equality comparer.
    /// </summary>
    [Obsolete("Consider to use a " + nameof(SymbolEqualityComparer) + " instead of this.")]
    public class TypeSymbolEqualityComparer : IEqualityComparer<ITypeSymbol>
    {
        public static readonly TypeSymbolEqualityComparer Entity = new();

        private TypeSymbolEqualityComparer()
        {
        }

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

            if (x.ToGlobalDisplayString() == y.ToGlobalDisplayString())
            {
                return true;
            }

            return false;
        }

        public int GetHashCode([DisallowNull] ITypeSymbol obj)
        {
            return obj.ToGlobalDisplayString().GetHashCode();
        }
    }
}
