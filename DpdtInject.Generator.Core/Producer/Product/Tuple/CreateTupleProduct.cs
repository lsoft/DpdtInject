using DpdtInject.Injector;
using DpdtInject.Injector.Helper;

namespace DpdtInject.Generator.Core.Producer.Product.Tuple
{
    public class CreateTupleProduct : IWritable
    {
        private readonly IWritablePair[] _tuplesParts;

        public CreateTupleProduct(
            params IWritablePair[] tuplesParts
            )
        {
            _tuplesParts = tuplesParts;
        }

        public void Write(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.Write("new System.Tuple<");

            _tuplesParts.IterateWithLastSignal(
                t => t.WriteItem0(writer, sng),
                () => writer.Write(", ")
                );

            writer.Write(">(");

            _tuplesParts.IterateWithLastSignal(
                t => t.WriteItem1(writer, sng),
                () => writer.Write(", ")
                );

            writer.Write(")");
        }
    }

}
