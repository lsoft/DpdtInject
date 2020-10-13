using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Producer.Product;
using DpdtInject.Generator.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Producer.Factory
{
    internal class FactoryProducer
    {
        private readonly ITypeInfoProvider _typeInfoProvider;
        
        public ClusterBindings ClusterBindings
        {
            get;
        }

        public FactoryProducer(
            ITypeInfoProvider typeInfoProvider,
            ClusterBindings clusterBindings
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            if (clusterBindings is null)
            {
                throw new ArgumentNullException(nameof(clusterBindings));
            }

            _typeInfoProvider = typeInfoProvider;
            ClusterBindings = clusterBindings;
        }

        internal IEnumerable<FactoryProduct> Produce()
        {
            foreach(var bindingContainer in ClusterBindings.BindingContainers)
            {
                if(!bindingContainer.ToFactory)
                {
                    continue;
                }

                yield return new FactoryProduct(
                    bindingContainer.BindToType,
                    $@"
namespace DpdtInject.Tests.Factory.Simple0
{{
    public partial class AFactory : IAFactory
    {{
        public IA Create()
        {{
            return new A();
        }}
    }}
}}
");
            }

            yield break;
        }
    }
}
