using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.ClassProducer.Product
{
    public interface IProducedClassProduct
    {
        public ITypeSymbol BindToType
        {
            get;
        }

        string GetSourceCode();
    }

}
