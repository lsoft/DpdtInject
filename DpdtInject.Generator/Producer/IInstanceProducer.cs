using DpdtInject.Generator.Producer.Product;

namespace DpdtInject.Generator.Producer
{
    internal interface IInstanceProducer
    {
        InstanceProduct Produce();
    }
}