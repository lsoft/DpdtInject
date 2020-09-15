using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

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
            BindingsContainer bindingsContainer
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
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

            Groups = new InstanceContainerGeneratorGroups(_instanceContainerGenerators);
        }


        public IReadOnlyList<InstanceContainerGenerator> GetBindWith(
            string bindFromTypeFullName
            )
        {
            if (bindFromTypeFullName is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypeFullName));
            }

            return
                _instanceContainerGenerators.FindAll(icg => icg.BindingContainer.FromTypeFullNames.Contains(bindFromTypeFullName));
        }
    }

    public class InstanceContainerGeneratorGroups
    {
        public List<InstanceContainerGenerator> InstanceContainerGenerators
        {
            get;
        }

        public Dictionary<ITypeSymbol, List<InstanceContainerGenerator>> ContainerGroups
        {
            get;
        }

        public InstanceContainerGeneratorGroups(
            List<InstanceContainerGenerator> instanceContainerGenerators
            )
        {
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

            ContainerGroups = processorGroups;
            InstanceContainerGenerators = instanceContainerGenerators;
        }

    }

}
