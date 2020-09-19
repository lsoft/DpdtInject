using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingContainerGroups
    {
        private readonly Dictionary<ITypeSymbol, List<IBindingContainer>> _bindGroups;

        public List<IBindingContainer> BindingContainers
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

            BindingContainers = bindingContainers;

            _bindGroups = new Dictionary<ITypeSymbol, List<IBindingContainer>>(
                new TypeSymbolEqualityComparer()
                );
            NotBindParentGroups = new Dictionary<ITypeSymbol, List<IBindingContainer>>(
                new TypeSymbolEqualityComparer()
                );

            foreach (var bc in bindingContainers)
            {
                foreach (var bindFromType in bc.BindFromTypes)
                {
                    if (!_bindGroups.ContainsKey(bindFromType))
                    {
                        _bindGroups[bindFromType] = new List<IBindingContainer>();
                    }

                    _bindGroups[bindFromType].Add(bc);
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

        }

        public bool TryGetRegisteredBindingContainers(
            ITypeSymbol type,
            bool includeWrappers,
            out IReadOnlyList<IBindingContainer> result
            )
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var rresult = new List<IBindingContainer>();

            if(_bindGroups.TryGetValue(type, out var list))
            {
                rresult.AddRange(list);
            }

            if (includeWrappers)
            {
                if (type.TryDetectWrapperType(out var wrapperType, out var internalType))
                {
                    if (_bindGroups.TryGetValue(internalType, out var wrappedList))
                    {
                        rresult.AddRange(wrappedList);
                    }
                }
            }

            result = rresult;
            return rresult.Count > 0;
        }
    }
}
