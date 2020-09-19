using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DpdtInject.Generator.Producer.Blocks.Binding
{
    public class InstanceContainerGeneratorsContainer
    {
        private readonly List<InstanceContainerGenerator> _instanceContainerGenerators;

        public BindingsContainer BindingsContainer
        {
            get;
        }

        public IReadOnlyList<InstanceContainerGenerator> InstanceContainerGenerators => _instanceContainerGenerators;

        public InstanceContainerGeneratorGroups Groups
        {
            get;
        }

        public InstanceContainerGeneratorsContainer(
            IDiagnosticReporter diagnosticReporter,
            Compilation compilation,
            BindingsContainer bindingsContainer
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

            BindingsContainer = bindingsContainer;

            _instanceContainerGenerators = new List<InstanceContainerGenerator>();
            foreach (var bindingContainer in bindingsContainer.BindingContainers)
            {
                _instanceContainerGenerators.Add(
                    new InstanceContainerGenerator(
                        diagnosticReporter,
                        bindingsContainer,
                        bindingContainer
                        ));
            }

            Groups = new InstanceContainerGeneratorGroups(
                compilation,
                _instanceContainerGenerators
                );
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

            foreach(var (wrapperType, wrapperSymbol) in Groups.GetRegisteredKeys(true))
            {
                clauses.Add(
                    $"new Tuple<Type, Func<object>>( typeof({wrapperSymbol.GetFullName()}), _provider.{providerMethodNamePrefix}_{wrapperSymbol.GetFullName().ConvertDotLessGreatherToGround()}{wrapperType.GetPostfix()} )"
                    );
            }

            return string.Join(",", clauses);
        }
    }

    public class InstanceContainerGeneratorGroups
    {
        private readonly Compilation _compilation;
        private readonly Dictionary<ITypeSymbol, List<InstanceContainerGenerator>> _containerGroups;

        public List<InstanceContainerGenerator> InstanceContainerGenerators
        {
            get;
        }

        public InstanceContainerGeneratorGroups(
            Compilation compilation,
            List<InstanceContainerGenerator> instanceContainerGenerators
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (instanceContainerGenerators is null)
            {
                throw new ArgumentNullException(nameof(instanceContainerGenerators));
            }

            var processorGroups = new Dictionary<ITypeSymbol, List<InstanceContainerGenerator>>(
                new TypeSymbolEqualityComparer()
                );

            foreach (var icg in instanceContainerGenerators)
            {
                foreach (var bindFromType in icg.BindFromTypes)
                {
                    if (!processorGroups.ContainsKey(bindFromType))
                    {
                        processorGroups[bindFromType] = new List<InstanceContainerGenerator>();
                    }

                    processorGroups[bindFromType].Add(icg);
                }
            }

            _containerGroups = processorGroups;
            _compilation = compilation;
            InstanceContainerGenerators = instanceContainerGenerators;
        }

        public IReadOnlyCollection<(DpdtArgumentWrapperTypeEnum, ITypeSymbol)> GetRegisteredKeys(bool withWrappers)
        {
            var result = new HashSet<(DpdtArgumentWrapperTypeEnum, ITypeSymbol)>();

            foreach (var key in _containerGroups.Keys)
            {
                result.Add((DpdtArgumentWrapperTypeEnum.None, key));

                if (withWrappers)
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

        public bool TryGetRegisteredGenerators(
            ITypeSymbol type,
            bool withWrappers,
            out IReadOnlyList<InstanceContainerGenerator> result
            )
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var rresult = new List<InstanceContainerGenerator>();

            if (_containerGroups.TryGetValue(type, out var list))
            {
                rresult.AddRange(list);
            }

            if (withWrappers)
            {
                if (type.TryDetectWrapperType(out var wrapperType, out var internalType))
                {
                    if (_containerGroups.TryGetValue(internalType, out var wrappedList))
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
