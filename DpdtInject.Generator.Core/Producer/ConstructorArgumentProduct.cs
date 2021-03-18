using System;

namespace DpdtInject.Generator.Core.Producer
{
    public class ConstructorArgumentProduct
    {
        public static readonly ConstructorArgumentProduct Empty = new(string.Empty);

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
    }
}
