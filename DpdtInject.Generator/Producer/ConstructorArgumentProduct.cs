using System;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer
{
    public class ConstructorArgumentProduct
    {
        public string ResolveConstructorArgumentClause
        {
            get;
        }

        public ConstructorArgumentProduct(
            string resolveConstructorArgumentClause
            )
        {
            if (resolveConstructorArgumentClause is null)
            {
                throw new ArgumentNullException(nameof(resolveConstructorArgumentClause));
            }

            ResolveConstructorArgumentClause = resolveConstructorArgumentClause;
        }

        public static readonly ConstructorArgumentProduct Empty = new(string.Empty);
    }
}
