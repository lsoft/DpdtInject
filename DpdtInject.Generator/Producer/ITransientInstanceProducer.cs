using DpdtInject.Generator.Producer.Product;

namespace DpdtInject.Generator.Producer
{
    internal interface ITransientInstanceProducer
    {
        InstanceProduct Produce();
    }
}