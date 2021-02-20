using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Producer
{
    internal static class LocalHelper
    {
        public static (List<ConstructorArgumentProduct>, List<UnknownTypeProduct>) Produce(
            this List<ConstructorArgumentProducer> constructorArgumentProducers
            )
        {
            if (constructorArgumentProducers is null)
            {
                throw new ArgumentNullException(nameof(constructorArgumentProducers));
            }

            List<ConstructorArgumentProduct> caps = new();
            List<UnknownTypeProduct> utps = new();
            foreach (var constructorArgumentProducer in constructorArgumentProducers)
            {
                var cap = constructorArgumentProducer.Produce(out var utp);

                if (ReferenceEquals(cap, ConstructorArgumentProduct.Empty))
                {
                    continue;
                }

                caps.Add(cap);

                if (utp is not null)
                {
                    utps.Add(utp);
                }
            }

            return (caps, utps);
        }
    }
}
