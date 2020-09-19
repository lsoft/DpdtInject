using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingContainerGroups
    {
        public List<IBindingContainer> BindingContainers
        {
            get;
        }

        public Dictionary<ITypeSymbol, List<IBindingContainer>> BindGroups
        {
            get;
        }

        public Dictionary<ITypeSymbol, List<IBindingContainer>> NotBindParentGroups
        {
            get;
        }

        public BindingContainerGroups(
            List<IBindingContainer> bindingContainers
            )
        {
            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            BindGroups = new Dictionary<ITypeSymbol, List<IBindingContainer>>(
                new TypeSymbolEqualityComparer()
                );
            NotBindParentGroups = new Dictionary<ITypeSymbol, List<IBindingContainer>>(
                new TypeSymbolEqualityComparer()
                );

            foreach (var bc in bindingContainers)
            {
                foreach (var bindFromType in bc.BindFromTypes)
                {
                    if (!BindGroups.ContainsKey(bindFromType))
                    {
                        BindGroups[bindFromType] = new List<IBindingContainer>();
                    }

                    BindGroups[bindFromType].Add(bc);
                }

                foreach(var cat in bc.NotBindConstructorArgumentTypes)
                {
                    if (!NotBindParentGroups.ContainsKey(cat))
                    {
                        NotBindParentGroups[cat] = new List<IBindingContainer>();
                    }

                    NotBindParentGroups[cat].Add(bc);
                }
            }

            BindingContainers = bindingContainers;
        }

    }
}
