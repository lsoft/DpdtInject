using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingContainerCluster
    {
        private readonly List<IBindingContainer> _bindingContainers;
        private readonly HashSet<ITypeSymbol> _bindsFrom;
        private readonly Dictionary<ITypeSymbol, BindingContainerGroup> _bindingContainerGroups;

        public ITypeSymbol DeclaredClusterType
        {
            get;
        }

        public bool IsRootCluster => DeclaredClusterType.BaseType!.GetFullName() == "System.Object";

        public IReadOnlyCollection<ITypeSymbol> BindsFrom => _bindsFrom;

        public IReadOnlyList<IBindingContainer> BindingContainers => _bindingContainers;

        public IReadOnlyDictionary<ITypeSymbol, BindingContainerGroup> BindingContainerGroups => _bindingContainerGroups;

        public Dictionary<ITypeSymbol, List<IBindingContainer>> NotBindParents
        {
            get;
        }

        public BindingContainerCluster(
            ITypeSymbol declaredClusterType,
            List<IBindingContainer> bindingContainers
            )
        {
            if (declaredClusterType is null)
            {
                throw new ArgumentNullException(nameof(declaredClusterType));
            }

            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            DeclaredClusterType = declaredClusterType;
            _bindingContainers = bindingContainers;

            _bindsFrom = new HashSet<ITypeSymbol>(
                new TypeSymbolEqualityComparer()
                );
            _bindingContainerGroups = new Dictionary<ITypeSymbol, BindingContainerGroup>(
                new TypeSymbolEqualityComparer()
                );
            NotBindParents = new Dictionary<ITypeSymbol, List<IBindingContainer>>(
                new TypeSymbolEqualityComparer()
                );

            foreach (var bc in bindingContainers)
            {
                foreach (var bindFromType in bc.BindFromTypes)
                {
                    _bindsFrom.Add(bindFromType);

                    if (!_bindingContainerGroups.ContainsKey(bindFromType))
                    {
                        _bindingContainerGroups[bindFromType] = new BindingContainerGroup(bindFromType);
                    }

                    _bindingContainerGroups[bindFromType].Add(bc);
                }

                foreach (var cat in bc.NotBindConstructorArgumentTypes)
                {
                    if (!NotBindParents.ContainsKey(cat))
                    {
                        NotBindParents[cat] = new List<IBindingContainer>();
                    }

                    NotBindParents[cat].Add(bc);
                }
            }

        }

        public bool TryGetRegisteredBindingGroups(
            ITypeSymbol type,
            bool includeWrappers,
            out IReadOnlyList<BindingContainerGroup> result
            )
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var rresult = new List<BindingContainerGroup>();

            if(_bindingContainerGroups.TryGetValue(type, out var group))
            {
                rresult.Add(group);
            }

            if (includeWrappers)
            {
                if (type.TryDetectWrapperType(out var wrapperType, out var internalType))
                {
                    if (_bindingContainerGroups.TryGetValue(internalType, out var wrapperGroup))
                    {
                        rresult.Add(wrapperGroup);
                    }
                }
            }

            result = rresult;
            return rresult.Count > 0;
        }
    }
}
