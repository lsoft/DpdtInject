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

        public Dictionary<ITypeSymbol, List<BindingContainer>> BindGroups
        {
            get;
        }

        public Dictionary<ITypeSymbol, List<BindingContainer>> NotBindParentGroups
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

            BindGroups = new Dictionary<ITypeSymbol, List<BindingContainer>>(
                new TypeSymbolEqualityComparer()
                );
            NotBindParentGroups = new Dictionary<ITypeSymbol, List<BindingContainer>>(
                new TypeSymbolEqualityComparer()
                );

            foreach (var bc in bindingContainers)
            {
                foreach (var bindFromType in bc.BindFromTypes)
                {
                    if (!BindGroups.ContainsKey(bindFromType))
                    {
                        BindGroups[bindFromType] = new List<BindingContainer>();
                    }

                    BindGroups[bindFromType].Add(bc);
                }

                foreach(var cat in bc.NotBindConstructorArgumentTypes)
                {
                    if (!NotBindParentGroups.ContainsKey(cat))
                    {
                        NotBindParentGroups[cat] = new List<BindingContainer>();
                    }

                    NotBindParentGroups[cat].Add(bc);
                }
            }

            BindingContainers = bindingContainers;
        }

    }
}
