using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.Product
{
    public class CreateTupleProduct

    {
        public (ITypeSymbol, string)[] TuplesParts
        {
            get;
        }

        public CreateTupleProduct(
            params (ITypeSymbol, string)[] tuplesParts
            )
        {
            TuplesParts = tuplesParts;
        }

        public string GetProduct()
        {
            var clause = $@"
new System.Tuple<{TuplesParts.Join(p => p.Item1.ToDisplayString(), ",")}>(
    {TuplesParts.Join(p => p.Item2, ",")}
    )
";

            return clause;
        }
    }
}
