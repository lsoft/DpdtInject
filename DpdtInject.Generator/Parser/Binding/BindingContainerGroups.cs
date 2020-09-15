using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingContainerGroups
    {
        public List<BindingContainer> BindingContainers
        {
            get;
        }

        public Dictionary<ITypeSymbol, List<BindingContainer>> ContainerGroups
        {
            get;
        }

        public BindingContainerGroups(
            List<BindingContainer> bindingContainers
            )
        {
            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            var processorGroups = new Dictionary<ITypeSymbol, List<BindingContainer>>(
                new TypeSymbolEqualityComparer()
                );

            foreach (var bc in bindingContainers)
            {
                foreach (var bindFromType in bc.BindFromTypes)
                {
                    if (!processorGroups.ContainsKey(bindFromType))
                    {
                        processorGroups[bindFromType] = new List<BindingContainer>();
                    }

                    processorGroups[bindFromType].Add(bc);
                }
            }

            ContainerGroups = processorGroups;
            BindingContainers = bindingContainers;
        }

    }
}
