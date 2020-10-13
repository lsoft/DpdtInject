using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Producer.Product;
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
        private readonly Compilation _compilation;
        
        public ClusterBindings ClusterBindings
        {
            get;
        }

        public FactoryProducer(
            Compilation compilation,
            ClusterBindings clusterBindings
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (clusterBindings is null)
            {
                throw new ArgumentNullException(nameof(clusterBindings));
            }

            _compilation = compilation;
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
            }

            yield break;
            //throw new NotImplementedException();
        }
    }
}
