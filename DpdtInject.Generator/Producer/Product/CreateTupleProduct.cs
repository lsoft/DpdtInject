using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
