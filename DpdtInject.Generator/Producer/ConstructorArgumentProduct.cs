using System;

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

        public static ConstructorArgumentProduct Empty = new ConstructorArgumentProduct(string.Empty);
    }
}
