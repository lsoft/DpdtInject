using DpdtInject.Generator.Core.Producer.Product;

namespace DpdtInject.Generator.Core.Producer
{
    internal interface IInstanceProducer
    {
        InstanceProduct Produce();
    }
}