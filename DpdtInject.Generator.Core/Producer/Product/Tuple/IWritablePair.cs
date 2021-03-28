using DpdtInject.Injector;
using DpdtInject.Generator.Core.Producer.Product;

namespace DpdtInject.Generator.Core.Producer.Product.Tuple
{
    public interface IWritablePair
    {
        void WriteItem0(IndentedTextWriter2 writer, ShortTypeNameGenerator sng);
        void WriteItem1(IndentedTextWriter2 writer, ShortTypeNameGenerator sng);
    }

}
