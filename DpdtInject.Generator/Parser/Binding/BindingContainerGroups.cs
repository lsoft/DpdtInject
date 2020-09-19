using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingContainerGroups
    {
        private readonly Dictionary<ITypeSymbol, List<IBindingContainer>> _bindGroups;
        private readonly Compilation _compilation;

        public List<IBindingContainer> BindingContainers
        {
            get;
        }

        public Dictionary<ITypeSymbol, List<IBindingContainer>> NotBindParentGroups
        {
            get;
        }

        public BindingContainerGroups(
            Compilation compilation,
            List<IBindingContainer> bindingContainers
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

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
            _compilation = compilation;
            BindingContainers = bindingContainers;
        }

        public IReadOnlyList<ITypeSymbol> GetRegisteredKeys(bool withWrappers)
        {
            var result = new List<ITypeSymbol>();

            foreach (var key in _bindGroups.Keys)
            {
                result.Add(key);

                if (key.TryDetectWrapperType(out var _, out var _))
                {
                    //Func<T> registered, probably we does not want to register Func<Func<T>>
                    continue;
                }

                if (withWrappers)
                {
                    foreach (var (wrapperType, wrapperSymbol) in key.GenerateWrapperTypes(_compilation))
                    {
                        result.Add(wrapperSymbol);
                    }
                }
            }

            return result;
        }

        public bool TryGetRegisteredBindingContainers(
            ITypeSymbol type,
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

            if(type.TryDetectWrapperType(out var wrapperType, out var internalType))
            {
                if (_bindGroups.TryGetValue(internalType, out var wrappedList))
                {
                    rresult.AddRange(wrappedList);
                }
            }

            result = rresult;
            return rresult.Count > 0;
        }

        public bool IsTypeRegistered(
            ITypeSymbol type,
            out DpdtArgumentWrapperTypeEnum wrapperType,
            out ITypeSymbol? internalType
            )
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if(this._bindGroups.ContainsKey(type))
            {
                wrapperType = DpdtArgumentWrapperTypeEnum.None;
                internalType = null;
                return true;
            }

            return
                type.TryDetectWrapperType(
                    out wrapperType,
                    out internalType
                    );
        }

    }
}
