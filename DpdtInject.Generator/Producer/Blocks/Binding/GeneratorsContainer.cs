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
    public class GeneratorTree
    {
        public GeneratorTreeJoint Joint
        {
            get;
        }

        public GeneratorCluster JointPayload => Joint.JointPayload;

        public GeneratorTree(
            GeneratorTreeJoint joint
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            Joint = joint;
        }

        internal void Apply(
            Action<GeneratorTreeJoint> action
            )
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Action<TreeJoint<GeneratorCluster>> action2 = (payload) => action((GeneratorTreeJoint)payload);

            Joint.Apply(action2);
        }

        public void Apply(
            Action<GeneratorCluster> action
            )
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Joint.Apply(action);
        }

        public bool TryFindInItsParents(
            Func<GeneratorCluster, bool> predicate,
            [NotNullWhen(true)] out GeneratorTreeJoint? foundJoint
            )
        {
            var r = 
                Joint.TryFindInItsParents(
                    predicate,
                    out var foundJoint2
                    );

            if(!r)
            {
                foundJoint = null;
                return false;
            }

            foundJoint = (GeneratorTreeJoint)foundJoint2!;
            return true;
        }
    }

    public class GeneratorTreeJoint : TreeJoint<GeneratorCluster>
    {
        public GeneratorTreeJoint(GeneratorCluster jointPayload)
            : base(jointPayload)
        {
        }

        public IReadOnlyList<Point3> GenerateChildPoints()
        {
            var result = new List<Point3>();

            this.Apply(
                joint =>
                {
                    foreach (var pair in joint.JointPayload.GeneratorGroups)
                    {
                        foreach (var generator in pair.Value.Generators)
                        {
                            foreach (var ca in generator.BindingContainer.ConstructorArguments.Where(ca => !ca.DefineInBindNode))
                            {
                                if (ca.Type is null)
                                {
                                    throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, $"ca.Type is null somehow");
                                }

                                var point3 = new Point3(
                                    this,
                                    generator,
                                    ca.Type
                                    );

                                result.Add(point3);
                            }
                        }
                    }
                });

            return result;
        }

    }


    public class Point2
    {
        public TreeJoint<GeneratorCluster> Joint
        {
            get;
        }
        
        public Generator Generator
        {
            get;
        }

        public Point2(
            TreeJoint<GeneratorCluster> joint,
            Generator generator
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            if (generator is null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            Joint = joint;
            Generator = generator;
        }

    }

    public class Point3
    {
        public TreeJoint<GeneratorCluster> Joint
        {
            get;
        }

        public Generator Generator
        {
            get;
        }
        public ITypeSymbol TypeSymbol
        {
            get;
        }

        public Point3(
            TreeJoint<GeneratorCluster> joint,
            Generator generator,
            ITypeSymbol typeSymbol
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            if (generator is null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            if (typeSymbol is null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            Joint = joint;
            Generator = generator;
            TypeSymbol = typeSymbol;
        }

    }

    public static class GeneratorTreeHelper
    {
        public static bool TryFindChildren(
            this Point3 point3,
            out IReadOnlyList<Point2> results
            )
        {
            if (point3 is null)
            {
                throw new ArgumentNullException(nameof(point3));
            }

            var result = new List<Point2>();

            FindChildren(point3, ref result);

            results = result;
            return result.Count > 0;
        }

        private static void FindChildren(
            Point3 point3,
            ref List<Point2> results
            )
        {
            if (point3 is null)
            {
                throw new ArgumentNullException(nameof(point3));
            }

            if (results is null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            if (!point3.Joint.JointPayload.TryGetRegisteredGeneratorGroups(
                point3.TypeSymbol,
                true,
                out var groups))
            {
                //no children found in current joint, move to parent joint

                if(point3.Joint.IsRoot)
                {
                    return;
                }

                var parentPoint3 = new Point3(
                    point3.Joint.Parent!,
                    point3.Generator,
                    point3.TypeSymbol
                    );

                FindChildren(
                    parentPoint3,
                    ref results
                    );
            }
            else
            {
                //we have children in this joint (cluster), no need to scan parent joints(clusters)

                foreach (var group in groups)
                {
                    foreach (var generator in group.Generators)
                    {
                        var rItem = new Point2(
                            point3.Joint,
                            generator
                            );

                        results.Add(rItem);
                    }
                }
            }
        }
    }



    public class GeneratorsContainer
    {
        private readonly List<Generator> _generators;
        private readonly HashSet<ITypeSymbol>  _allRegisteredTypes;

        public BindingsContainer BindingsContainer
        {
            get;
        }

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

            _generators = new List<Generator>();
            _allRegisteredTypes = new HashSet<ITypeSymbol>(
                new TypeSymbolEqualityComparer()
                );

            GeneratorTree.Apply(
                cluster =>
                {
                    foreach(var pair in cluster.GeneratorGroups)
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

        internal string GetReinventedContainerArgument(
            string providerMethodNamePrefix
            )
        {
            if (providerMethodNamePrefix is null)
            {
                throw new ArgumentNullException(nameof(providerMethodNamePrefix));
            }

            var clauses = new List<string>();

            foreach(var (wrapperType, wrapperSymbol) in GeneratorTree.JointPayload.GetRegisteredKeys(true))
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

            new CycleChecker(GeneratorTree.JointPayload)
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



            //foreach (var generator in _generators)
            //{
            //    AnalyzeForSingletonTakesTransientPrivate(
            //        diagnosticReporter,
            //        generator,
            //        new HashSet<Generator>()
            //        );
            //}
        }

        //private void AnalyzeForSingletonTakesTransientPrivate(
        //    IDiagnosticReporter diagnosticReporter,
        //    Generator parent,
        //    HashSet<Generator> processed
        //    )
        //{
        //    //if (diagnosticReporter is null)
        //    //{
        //    //    throw new ArgumentNullException(nameof(diagnosticReporter));
        //    //}

        //    //if (parent is null)
        //    //{
        //    //    throw new ArgumentNullException(nameof(parent));
        //    //}

        //    //if (processed is null)
        //    //{
        //    //    throw new ArgumentNullException(nameof(processed));
        //    //}

        //    //if (processed.Contains(parent))
        //    //{
        //    //    //circular dependency found
        //    //    //do not check this binding because of it's invalid a priori
        //    //    diagnosticReporter.ReportWarning(
        //    //        $"Searching for singleton-transient relationship has been skipped.",
        //    //        $"Searching for singleton-transient relationship has been skipped, because of circular dependency found with {parent.BindingContainer.TargetRepresentation}."
        //    //        );

        //    //    return;
        //    //}

        //    //processed.Add(parent);

        //    //foreach (var ca in parent.BindingContainer.ConstructorArguments.Where(j => !j.DefineInBindNode))
        //    //{
        //    //    if (ca.Type is null)
        //    //    {
        //    //        throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, $"ca.Type is null somehow");
        //    //    }

        //    //    if (!Groups.TryGetRegisteredGenerators(ca.Type!, true, out var children))
        //    //    {
        //    //        throw new DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, $"Found unknown binding [{ca.Type.GetFullName()}] from constructor of [{parent.BindingContainer.TargetRepresentation}]", ca.Type.Name);
        //    //    }

        //    //    foreach (var child in children)
        //    //    {
        //    //        if (parent.BindingContainer.Scope.In(BindScopeEnum.Singleton))
        //    //        {
        //    //            if (child.BindingContainer.Scope.In(BindScopeEnum.Transient))
        //    //            {
        //    //                diagnosticReporter.ReportWarning(
        //    //                    $"Singleton-transient relationship has been found.",
        //    //                    $"Searching for singleton-transient relationship has been found: singleton parent [{parent.BindingContainer.TargetRepresentation}] takes transient child [{child.BindingContainer.TargetRepresentation}]."
        //    //                    );
        //    //            }
        //    //        }

        //    //        AnalyzeForSingletonTakesTransientPrivate(
        //    //            diagnosticReporter,
        //    //            child,
        //    //            new HashSet<Generator>(processed)
        //    //            );
        //    //    }
        //    //}
        //}

    }

}
