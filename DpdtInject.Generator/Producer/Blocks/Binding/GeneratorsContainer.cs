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
    public class GeneratorsContainer
    {
        private readonly List<Generator> _generators;
        private readonly List<GeneratorCluster> _generatorClusters;
        private readonly HashSet<ITypeSymbol>  _allRegisteredTypes;

        public BindingsContainer BindingsContainer
        {
            get;
        }

        public IReadOnlyList<GeneratorCluster> GeneratorClusters => _generatorClusters;

        public IReadOnlyList<Generator> Generators => _generators;

        public IReadOnlyCollection<ITypeSymbol> AllRegisteredTypes => _allRegisteredTypes;

        public GeneratorTree GeneratorTree
        {
            get;
        }

        public GeneratorsContainer(
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

            GeneratorTree = new GeneratorTree(
                bindingsContainer.BindingClusterTree.ClusterJoint.ConvertTo<GeneratorTreeJoint, GeneratorCluster>(
                    joint => new GeneratorTreeJoint(
                    new GeneratorCluster(
                        diagnosticReporter,
                        compilation,
                        joint.JointPayload
                        )
                    )
                ));
            GeneratorTree.BuildFlags();

            _generators = new List<Generator>();
            _generatorClusters = new List<GeneratorCluster>();
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

        //internal string GetReinventedContainerArgument(
        //    string providerMethodNamePrefix
        //    )
        //{
        //    if (providerMethodNamePrefix is null)
        //    {
        //        throw new ArgumentNullException(nameof(providerMethodNamePrefix));
        //    }

        //    var clauses = new List<string>();

        //    foreach(var (wrapperType, wrapperSymbol) in GeneratorTree.JointPayload.GetRegisteredKeys(true))
        //    {
        //        clauses.Add(
        //            $"new Tuple<Type, Func<object>>( typeof({wrapperSymbol.GetFullName()}), _provider.{providerMethodNamePrefix}_{wrapperSymbol.GetFullName().ConvertDotLessGreatherToGround()}{wrapperType.GetPostfix()} )"
        //            );
        //    }

        //    return string.Join(",", clauses);
        //}

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
