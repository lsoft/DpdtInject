using DpdtInject.Generator.Producer.ClassProducer.Product;

namespace DpdtInject.Generator.Producer.ClassProducer
{
    internal interface IClassProducer
    {
        IProducedClassProduct GenerateProduct(
            );
    }
}
