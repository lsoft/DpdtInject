using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.Graph;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
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

        internal void AnalyzeForCircularDependencies(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            new CycleChecker(Groups)
                .CheckForCycles(diagnosticReporter)
                ;
        }

        internal void AnalyzeForUnknownBindings(
            IDiagnosticReporter diagnosticReporter
            )
        {
            foreach (var generator in _instanceContainerGenerators)
            {
                foreach (var ca in generator.BindingContainer.ConstructorArguments.Where(j => !j.DefineInBindNode))
                {
                    if (ca.Type is null)
                    {
                        throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, $"ca.Type is null somehow");
                    }

                    if (Groups.GetRegisteredKeys(true).All(p => !SymbolEqualityComparer.Default.Equals(p.Item2, ca.Type)))
                    {
                        throw new DpdtException(
                            DpdtExceptionTypeEnum.NoBindingAvailable,
                            $"Found unknown binding [{ca.Type!.GetFullName()}] from constructor of [{generator.BindingContainer.TargetRepresentation}]",
                            ca.Type.Name
                            );
                    }
                }
            }
        }

        internal void AnalyzeForSingletonTakesTransient(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            foreach (var generator in _instanceContainerGenerators)
            {
                AnalyzeForSingletonTakesTransientPrivate(
                    diagnosticReporter,
                    generator,
                    new HashSet<InstanceContainerGenerator>()
                    );
            }
        }

        private void AnalyzeForSingletonTakesTransientPrivate(
            IDiagnosticReporter diagnosticReporter,
            InstanceContainerGenerator parent,
            HashSet<InstanceContainerGenerator> processed
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (processed is null)
            {
                throw new ArgumentNullException(nameof(processed));
            }

            if (processed.Contains(parent))
            {
                //circular dependency found
                //do not check this binding because of it's invalid a priori
                diagnosticReporter.ReportWarning(
                    $"Searching for singleton-transient relationship has been skipped.",
                    $"Searching for singleton-transient relationship has been skipped, because of circular dependency found with {parent.BindingContainer.TargetRepresentation}."
                    );

                return;
            }

            processed.Add(parent);

            foreach (var ca in parent.BindingContainer.ConstructorArguments.Where(j => !j.DefineInBindNode))
            {
                if (ca.Type is null)
                {
                    throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, $"ca.Type is null somehow");
                }

                if (!Groups.TryGetRegisteredGenerators(ca.Type!, true, out var children))
                {
                    throw new DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, $"Found unknown binding [{ca.Type.GetFullName()}] from constructor of [{parent.BindingContainer.TargetRepresentation}]", ca.Type.Name);
                }

                foreach (var child in children)
                {
                    if (parent.BindingContainer.Scope.In(BindScopeEnum.Singleton))
                    {
                        if (child.BindingContainer.Scope.In(BindScopeEnum.Transient))
                        {
                            diagnosticReporter.ReportWarning(
                                $"Singleton-transient relationship has been found.",
                                $"Searching for singleton-transient relationship has been found: singleton parent [{parent.BindingContainer.TargetRepresentation}] takes transient child [{child.BindingContainer.TargetRepresentation}]."
                                );
                        }
                    }

                    AnalyzeForSingletonTakesTransientPrivate(
                        diagnosticReporter,
                        child,
                        new HashSet<InstanceContainerGenerator>(processed)
                        );
                }
            }
        }

    }

}
