using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingClusterTree
    {
        public const string RootName = "";

        public TreeJoint<BindingContainerCluster> ClusterJoint
        {
            get;
        }

        public BindingClusterTree(
            TreeJoint<BindingContainerCluster> clusterJoint
            )
        {
            if (clusterJoint is null)
            {
                throw new ArgumentNullException(nameof(clusterJoint));
            }

            ClusterJoint = clusterJoint;
        }

        public void BuildFlags(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            BuildFlagsInternal(
                diagnosticReporter,
                ClusterJoint
                );
        }

        public void BuildFlagsInternal(
            IDiagnosticReporter diagnosticReporter,
            TreeJoint<BindingContainerCluster> joint
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            foreach (var pair in joint.JointPayload.BindingContainerGroups)
            {
                foreach (var bindingContainer in pair.Value.BindingContainers)
                {
                    var atLeastOneChildIsConditional = false;
                    foreach (var ca in bindingContainer.ConstructorArguments.Where(j => !j.DefineInBindNode))
                    {
                        //check if any child is conditional
                        atLeastOneChildIsConditional = CheckForAtLeastOneChildIsConditionalInternal(
                            diagnosticReporter,
                            joint,
                            bindingContainer,
                            bindingContainer,
                            ca.Type!,
                            new HashSet<IBindingContainer>()
                            );

                        if(atLeastOneChildIsConditional)
                        {
                            break;
                        }
                    }

                    bindingContainer.AtLeastOneChildIsConditional = atLeastOneChildIsConditional;
                }
            }

            foreach (var childJoint in joint.Children)
            {
                BuildFlagsInternal(
                    diagnosticReporter,
                    childJoint
                    );
            }
        }

        private bool CheckForAtLeastOneChildIsConditionalInternal(
            IDiagnosticReporter diagnosticReporter,
            TreeJoint<BindingContainerCluster> joint,
            IBindingContainer rootContainer,
            IBindingContainer currentContainer,
            ITypeSymbol bindFrom,
            HashSet<IBindingContainer> processed
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            if (rootContainer is null)
            {
                throw new ArgumentNullException(nameof(rootContainer));
            }

            if (currentContainer is null)
            {
                throw new ArgumentNullException(nameof(currentContainer));
            }

            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            if (processed is null)
            {
                throw new ArgumentNullException(nameof(processed));
            }

            var cluster = joint.JointPayload;

            if(!cluster.TryGetRegisteredBindingGroups(bindFrom, true, out var groups))
            {
                //no children found in current joint, move to parent joint

                if (!joint.TryFindInItsParents(
                    cluster => cluster.BindsFrom.Contains(bindFrom),
                    out var parentJoint
                    ))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.NoBindingAvailable,
                        $"Found unknown binding [{bindFrom.GetFullName()}] from constructor of [{currentContainer.TargetRepresentation}] in the cluster [{cluster.Name}]",
                        bindFrom.Name
                        );
                }

                //search for children in parent joint (cluster)
                return
                    CheckForAtLeastOneChildIsConditionalInternal(
                        diagnosticReporter,
                        parentJoint,
                        rootContainer,
                        currentContainer,
                        bindFrom,
                        processed
                        );
            }

            if (processed.Contains(currentContainer))
            {
                //circular dependency found
                //do not check this binding because of it's invalid a priori
                diagnosticReporter.ReportWarning(
                    $"Searching for undeterministic resolution path (up) has been skipped.",
                    $"Searching for undeterministic resolution path (up) for [{rootContainer.TargetRepresentation}] has been skipped, because of circular dependency found."
                    );
                return false;
            }

            processed.Add(currentContainer);


            foreach (var group in groups)
            {
                foreach(var bindingContainer in group.BindingContainers)
                {
                    //we need to check only a children not a itself!
                    if(ReferenceEquals(bindingContainer, rootContainer))
                    {
                        continue;
                    }

                    if(bindingContainer.IsConditional)
                    {
                        return true;
                    }

                    foreach (var ca in bindingContainer.ConstructorArguments.Where(j => !j.DefineInBindNode))
                    {
                        if (ca.Type is null)
                        {
                            throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, $"ca.Type is null somehow");
                        }

                        //search for child in same joint (cluster)
                        if(CheckForAtLeastOneChildIsConditionalInternal(
                            diagnosticReporter,
                            joint,
                            rootContainer,
                            bindingContainer,
                            ca.Type,
                            new HashSet<IBindingContainer>(processed)
                            ))
                        {
                            return true;
                        }

                        //this child is non conditional, move to the next
                    }
                }
            }

            //all children are non conditional
            return false;
        }
    }



    public class BindingContainerGroup
    {
        private readonly List<IBindingContainer> _bindingContainers;

        public ITypeSymbol BindFrom
        {
            get;
        }

        public IReadOnlyList<IBindingContainer> BindingContainers => _bindingContainers;

        public BindingContainerGroup(
            ITypeSymbol bindFrom
            )
        {
            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            BindFrom = bindFrom;
            _bindingContainers = new List<IBindingContainer>();
        }

        public void Add(IBindingContainer bindingContainer)
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            _bindingContainers.Add(bindingContainer);
        }

    }

    public class BindingContainerCluster
    {
        private readonly List<IBindingContainer> _bindingContainers;
        private readonly HashSet<ITypeSymbol> _bindsFrom;
        private readonly Dictionary<ITypeSymbol, BindingContainerGroup> _bindingContainerGroups;

        public string Name
        {
            get;
        }

        public IReadOnlyCollection<ITypeSymbol> BindsFrom => _bindsFrom;

        public IReadOnlyList<IBindingContainer> BindingContainers => _bindingContainers;

        public IReadOnlyDictionary<ITypeSymbol, BindingContainerGroup> BindingContainerGroups => _bindingContainerGroups;

        public Dictionary<ITypeSymbol, List<IBindingContainer>> NotBindParents
        {
            get;
        }

        public BindingContainerCluster(
            string name,
            List<IBindingContainer> bindingContainers
            )
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            Name = name;
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
