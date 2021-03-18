using DpdtInject.Generator.Core.Producer.ClassProducer.Product;

namespace DpdtInject.Generator.Core.Producer.ClassProducer
{
    internal interface IClassProducer
    {
        IProducedClassProduct GenerateProduct(
            );
    }
}
