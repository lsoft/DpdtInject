using DpdtInject.Generator.Core.Binding;
using DpdtInject.Injector.Src.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Core.Producer
{
    public static class ConstructorArgumentHelper
    {
        public static IReadOnlyList<ConstructorArgumentProducer> ConvertToProducers(
            this IReadOnlyList<DetectedMethodArgument> constructorArguments,
            ClusterBindings clusterBindings,
            BindingExtender bindingExtender
            )
        {
            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }

            if (clusterBindings is null)
            {
                throw new ArgumentNullException(nameof(clusterBindings));
            }

            if (bindingExtender is null)
            {
                throw new ArgumentNullException(nameof(bindingExtender));
            }

            return constructorArguments.OrderBy(ca => ca.ArgumentIndex).Select(
                    bft => new ConstructorArgumentProducer(
                        clusterBindings,
                        bindingExtender,
                        bft
                        )
                    ).ToList();
        }

    }
}
