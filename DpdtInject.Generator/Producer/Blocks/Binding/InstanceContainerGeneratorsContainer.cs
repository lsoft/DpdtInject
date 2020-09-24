using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.Graph;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DpdtInject.Generator.Producer.Blocks.Binding
{
    public class InstanceContainerGeneratorsContainer

    {
        private readonly List<InstanceContainerGenerator> _generators;
        private readonly List<InstanceContainerGeneratorCluster> _generatorClusters;
        private readonly HashSet<ITypeSymbol>  _allRegisteredTypes;

        public BindingsContainer BindingsContainer
        {
            get;
        }

        public IReadOnlyList<InstanceContainerGeneratorCluster> GeneratorClusters => _generatorClusters;

        public IReadOnlyList<InstanceContainerGenerator> Generators => _generators;

        public IReadOnlyCollection<ITypeSymbol> AllRegisteredTypes => _allRegisteredTypes;

        public InstanceContainerGeneratorTree GeneratorTree
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

            if (bindingsContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingsContainer));
            }

            BindingsContainer = bindingsContainer;

            GeneratorTree = new InstanceContainerGeneratorTree(
                bindingsContainer.BindingClusterTree.ClusterJoint.ConvertTo2<InstanceContainerGeneratorTreeJoint, InstanceContainerGeneratorCluster>(
                    (parentJoint, toConvertJoint) => new InstanceContainerGeneratorTreeJoint(
                        parentJoint,
                        new InstanceContainerGeneratorCluster(
                            diagnosticReporter,
                            compilation,
                            toConvertJoint.JointPayload
                            )
                    )
                ));
            GeneratorTree.BuildFlags();

            _generators = new List<InstanceContainerGenerator>();
            _generatorClusters = new List<InstanceContainerGeneratorCluster>();
            _allRegisteredTypes = new HashSet<ITypeSymbol>(
                new TypeSymbolEqualityComparer()
                );

            GeneratorTree.Apply(
                cluster =>
                {
                    _generatorClusters.Add(cluster);

                    foreach (var pair in cluster.GeneratorGroups)
                    {
                        foreach(var generator in pair.Value.Generators)
                        {
                            _generators.Add(generator);
                            foreach (var bindFrom in generator.BindFromTypes)
                            {
                                _allRegisteredTypes.Add(bindFrom);
                            }
                        }
                    }
                }
                );


        }

        internal void AnalyzeForCircularDependencies(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            new CycleChecker(GeneratorTree.Joint)
                .CheckForCycles(diagnosticReporter)
                ;
        }

        internal void AnalyzeForUnknownBindings(
            IDiagnosticReporter diagnosticReporter
            )
        {
            //we need to check unknown bindings in the tree

            foreach(var point3 in GeneratorTree.Joint.GenerateChildPoints())
            {
                if (!point3.TryFindChildren(out var _))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.NoBindingAvailable,
                        $"Found unknown binding [{point3.TypeSymbol.GetFullName()}] from constructor of [{point3.Generator.BindingContainer.TargetRepresentation}]",
                        point3.TypeSymbol.GetFullName()
                        );
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

            foreach (var point3 in GeneratorTree.Joint.GenerateChildPoints())
            {
                if (point3.Generator.BindingContainer.Scope.In(BindScopeEnum.Singleton))
                {
                    if (point3.TryFindChildren(out var children))
                    {
                        foreach(var child in children)
                        {
                            if (child.Generator.BindingContainer.Scope.In(BindScopeEnum.Transient))
                            {
                                diagnosticReporter.ReportWarning(
                                    $"Singleton-transient relationship has been found.",
                                    $"Searching for singleton-transient relationship has been found: singleton parent [{point3.Generator.BindingContainer.TargetRepresentation}] takes transient child [{child.Generator.BindingContainer.TargetRepresentation}]."
                                    );
                            }
                        }
                    }
                }

            }
        }
    }

}
