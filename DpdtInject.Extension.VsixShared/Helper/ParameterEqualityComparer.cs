using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DpdtInject.Extension.Helper
{
    public class ParameterEqualityComparer : IEqualityComparer<IParameterSymbol>
    {
        public static readonly ParameterEqualityComparer Entity = new ParameterEqualityComparer();

        public bool Equals(IParameterSymbol x, IParameterSymbol y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null && y != null)
            {
                return false;
            }
            if (x != null && y == null)
            {
                return false;
            }

            if (x!.Name != y!.Name)
            {
                return false;
            }

            if (x.ContainingType != null && y.ContainingType != null)
            {
                if (x.ContainingType.ToDisplayString() != y.ContainingType.ToDisplayString())
                {
                    return false;
                }
            }

            if (x.ToDisplayString() != y.ToDisplayString())
            {
                return false;
            }

            return true;
        }

        public int GetHashCode(IParameterSymbol obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}
