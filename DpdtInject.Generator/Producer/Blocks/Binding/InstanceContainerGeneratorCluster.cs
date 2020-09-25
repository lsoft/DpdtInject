using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Binding
{
    public class InstanceContainerGeneratorCluster
    {
        private readonly Compilation _compilation;
        private readonly Dictionary<ITypeSymbol, InstanceContainerGeneratorGroup> _generatorGroups;
        private readonly List<InstanceContainerGenerator> _generators;

        internal ITypeSymbol DeclaredClusterType => BindingContainerCluster.DeclaredClusterType;

        public bool IsRootCluster => BindingContainerCluster.IsRootCluster;


        public BindingContainerCluster BindingContainerCluster
        {
            get;
        }

        public IReadOnlyDictionary<ITypeSymbol, InstanceContainerGeneratorGroup> GeneratorGroups => _generatorGroups;

        public IReadOnlyList<InstanceContainerGenerator> Generators => _generators;

        public IReadOnlyCollection<ITypeSymbol> BindsFrom => _generatorGroups.Keys;

        public InstanceContainerGeneratorCluster(
            IDiagnosticReporter diagnosticReporter,
            Compilation compilation,
            BindingContainerCluster bindingContainerCluster
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (bindingContainerCluster is null)
            {
                throw new ArgumentNullException(nameof(bindingContainerCluster));
            }

            _compilation = compilation;
            BindingContainerCluster = bindingContainerCluster;

            _generatorGroups = bindingContainerCluster.BindingContainerGroups.ToDictionary(
                g => g.Key,
                g => new InstanceContainerGeneratorGroup(
                    g.Key,
                    g.Value.BindingContainers.ConvertAll(
                        bc => new InstanceContainerGenerator(
                            diagnosticReporter,
                            bc
                            )
                        )
                    )
                );

            _generators = new List<InstanceContainerGenerator>();
            foreach(var pair in _generatorGroups)
            {
                _generators.AddRange(pair.Value.Generators);
            }
        }

        internal string GetReinventedContainerArgument(
            string providerMethodNamePrefix
            )
        {
            if (providerMethodNamePrefix is null)
            {
                throw new ArgumentNullException(nameof(providerMethodNamePrefix));
            }

            var clauses = new List<string>();

            foreach (var (wrapperType, wrapperSymbol) in GetRegisteredKeys(true))
            {
                clauses.Add(
                    $"new Tuple<Type, Func<object>>( typeof({wrapperSymbol.GetFullName()}), {providerMethodNamePrefix}_{wrapperSymbol.GetFullName().EscapeSpecialTypeSymbols()}{wrapperType.GetPostfix()} )"
                    );
            }

            return string.Join(",", clauses);
        }

        internal string PrepareDeclaredClusterType()
        {
            if(IsRootCluster)
            {
                return "null";
            }

            return $"typeof({DeclaredClusterType.Name})";
        }

        public IReadOnlyCollection<(DpdtArgumentWrapperTypeEnum, ITypeSymbol)> GetRegisteredKeys(bool includeWrappers)
        {
            var result = new HashSet<(DpdtArgumentWrapperTypeEnum, ITypeSymbol)>();

            foreach (var key in _generatorGroups.Keys)
            {
                result.Add((DpdtArgumentWrapperTypeEnum.None, key));

                if (includeWrappers)
                {
                    if (key.TryDetectWrapperType(out var _, out var _))
                    {
                        //Func<T> registered, probably we does not want to register Func<Func<T>>
                        continue;
                    }

                    foreach(var pair in key.GenerateWrapperTypes(_compilation))
                    {
                        result.Add(pair);
                    }
                }
            }

            return result;
        }

        public bool TryGetRegisteredGeneratorGroups(
            ITypeSymbol type,
            bool includeWrappers,
            out IReadOnlyList<InstanceContainerGeneratorGroup> result
            )
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var rresult = new List<InstanceContainerGeneratorGroup>();

            if (_generatorGroups.TryGetValue(type, out var group))
            {
                rresult.Add(group);
            }

            if (includeWrappers)
            {
                if (type.TryDetectWrapperType(out var wrapperType, out var wrapperSymbol))
                {
                    if (_generatorGroups.TryGetValue(wrapperSymbol, out var wrappedList))
                    {
                        rresult.Add(wrappedList);
                    }
                }
            }

            result = rresult;
            return rresult.Count > 0;
        }

    }

}
