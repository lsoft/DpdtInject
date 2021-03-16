using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.CodeDom.Compiler;

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

        internal void WriteProduct(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.Write("new System.Tuple<");

            TuplesParts.IterateWithLastSignal(
                t => writer.Write(t.Item1.ToDisplayString()),
                () => writer.Write(", ")
                );

            writer.Write(">(");

            TuplesParts.IterateWithLastSignal(
                t => writer.Write(t.Item2),
                () => writer.Write(", ")
                );

            writer.Write(")");
        }
    }
}
