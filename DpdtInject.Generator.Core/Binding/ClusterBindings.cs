using DpdtInject.Generator.Core.Graph;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Generator.Core.Binding
{
    public class ClusterBindings
    {
        public ITypeSymbol ClusterType
        {
            get;
        }

        public IReadOnlyList<IBindingContainer> BindingContainers
        {
            get;
        }

        public IReadOnlyList<BindingContainerExtender> BindingExtenders
        {
            get;
        }


        public Dictionary<ITypeSymbol, List<BindingContainerExtender>> NotBindParents
        {
            get;
        }
        public BindingExtenderBox Box
        {
            get;
        }

        public ClusterBindings(
            ITypeSymbol clusterType,
            IReadOnlyList<IBindingContainer> bindingContainers
            )
        {
            if (clusterType is null)
            {
                throw new ArgumentNullException(nameof(clusterType));
            }

            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            ClusterType = clusterType;
            BindingContainers = bindingContainers;

            BindingExtenders = bindingContainers.ConvertAll(c => new BindingContainerExtender(c));

            NotBindParents = new Dictionary<ITypeSymbol, List<BindingContainerExtender>>(
                SymbolEqualityComparer.Default
                );

            foreach (var extender in BindingExtenders)
            {
                foreach (var cat in extender.BindingContainer.NotBindConstructorArgumentTypes)
                {
                    if (!NotBindParents.ContainsKey(cat))
                    {
                        NotBindParents[cat] = new List<BindingContainerExtender>();
                    }

                    NotBindParents[cat].Add(extender);
                }
            }

            Box = new BindingExtenderBox(
                BindingExtenders
                );
        }

        internal void BuildFlags()
        {
            CheckForNeedToProcessResolutionContextInternal(
                );
        }

        private void CheckForNeedToProcessResolutionContextInternal(
            )
        {
            foreach (var pair in Box.Groups)
            {
                var group = pair.Value;
                foreach (var bindingExtender in group.BindingExtenders)
                {
                    {
                        var used = new HashSet<IBindingContainer>();
                        var itselfOrAnyChildNeedForContext = CheckForItselfOrAnyChildNeedForContext(
                            ref used,
                            Box,
                            bindingExtender.BindingContainer
                            );

                        bindingExtender.ItselfOrAnyChildNeedForContext = itselfOrAnyChildNeedForContext;
                    }

                    {
                        var used = new HashSet<IBindingContainer>();
                        var itselfOrAnyChildIsConditional = CheckForItselfOrAnyChildIsConditional(
                            ref used,
                            Box,
                            bindingExtender.BindingContainer
                            );

                        bindingExtender.ItselfOrAnyChildIsConditional = itselfOrAnyChildIsConditional;
                    }

                    {
                        var used = new HashSet<IBindingContainer>();
                        var hasUnresolvedChildren = CheckForHasUnresolvedChildren(
                            ref used,
                            Box,
                            bindingExtender.BindingContainer
                            );

                        bindingExtender.HasUnresolvedChildren = hasUnresolvedChildren;
                    }
                }
            }
        }
        public void Analyze(
            IDiagnosticReporter diagnosticReporter
            )
        {
            AnalyzeForCircularDependencies(diagnosticReporter);
            AnalyzeForMultipleUnconditionalChildExists(diagnosticReporter);
            AnalyzeForSingletonTakesCustomOrTransient(diagnosticReporter);
        }

        #region private

        private void AnalyzeForMultipleUnconditionalChildExists(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            foreach (var pair in this.Box.Groups.Shuffle())
            {
                var group = pair.Value;
                foreach (var bindingExtender in group.BindingExtenders)
                {
                    if (Box.TryGetChildren(bindingExtender.BindingContainer, true, out var cpairs))
                    {
                        var cgroups = (
                            from cpair in cpairs
                            where !cpair.ConstructorArgument.DefineInBindNode
                            group cpair by cpair.ConstructorArgument.Name into cgroup
                            select cgroup
                            ).ToList();

                        foreach(var cgroup in cgroups)
                        {
                            var cgpairs = cgroup.ToList();
                            var unconditionalCount = cgpairs.Count(p => !p.BindingExtender.BindingContainer.IsConditional);
                            if(unconditionalCount > 1)
                            {
                                diagnosticReporter.ReportError(
                                    $"Multiple unconditional children has been found.",
                                    $"There are multiple unconditional children for [{bindingExtender.BindingContainer.TargetRepresentation}] and its constructor argument [{cgroup.Key}]."
                                    );
                            }
                        }
                    }
                }
            }
        }

        private void AnalyzeForSingletonTakesCustomOrTransient(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            foreach(var pair in this.Box.Groups.Shuffle())
            {
                var group = pair.Value;
                foreach(var bindingExtender in group.BindingExtenders)
                {
                    if(bindingExtender.BindingContainer.Scope.In(BindScopeEnum.Singleton))
                    {
                        if (Box.TryGetChildren(bindingExtender.BindingContainer, true, out var cpairs))
                        {
                            foreach (var cpair in cpairs)
                            {
                                var childExtender = cpair.BindingExtender;

                                if (childExtender.BindingContainer.Scope.In(BindScopeEnum.Custom, BindScopeEnum.Transient))
                                {
                                    diagnosticReporter.ReportWarning(
                                        $"Singleton-{childExtender.BindingContainer.Scope} relationship has been found.",
                                        $"Searching for singleton-{childExtender.BindingContainer.Scope} relationship has been found: singleton parent [{bindingExtender.BindingContainer.TargetRepresentation}] takes {childExtender.BindingContainer.Scope} child [{childExtender.BindingContainer.TargetRepresentation}]."
                                        );
                                }
                            }
                        }
                    }
                }
            }
        }


        private bool CheckForHasUnresolvedChildren(
            ref HashSet<IBindingContainer> used,
            BindingExtenderBox box,
            IBindingContainer bindingContainer
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            if (used is null)
            {
                throw new ArgumentNullException(nameof(used));
            }

            if (box is null)
            {
                throw new ArgumentNullException(nameof(box));
            }

            if (used.Contains(bindingContainer))
            {
                //found cycle, skip this subtree as circular
                return false;
            }
            used.Add(bindingContainer);

            if (!box.TryGetChildren(bindingContainer, false, out var pairs))
            {
                //at least one child does not found
                return true;
            }

            foreach (var pair in pairs)
            {
                var used2 = new HashSet<IBindingContainer>(used);
                if (CheckForHasUnresolvedChildren(
                    ref used2,
                    box,
                    pair.BindingExtender.BindingContainer
                    ))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckForItselfOrAnyChildNeedForContext(
            ref HashSet<IBindingContainer> used,
            BindingExtenderBox box,
            IBindingContainer bindingContainer
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            if (used is null)
            {
                throw new ArgumentNullException(nameof(used));
            }

            if (box is null)
            {
                throw new ArgumentNullException(nameof(box));
            }

            if (used.Contains(bindingContainer))
            {
                //found cycle, skip this subtree as circular
                return false;
            }
            used.Add(bindingContainer);

            if (bindingContainer.Scope == BindScopeEnum.Custom)
            {
                return true;
            }

            if (box.TryGetChildren(bindingContainer, true, out var pairs))
            {
                foreach (var pair in pairs)
                {
                    var used2 = new HashSet<IBindingContainer>(used);
                    if (CheckForItselfOrAnyChildNeedForContext(
                        ref used2,
                        box,
                        pair.BindingExtender.BindingContainer
                        ))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckForItselfOrAnyChildIsConditional(
            ref HashSet<IBindingContainer> used,
            BindingExtenderBox bindingContainerBox,
            IBindingContainer bindingContainer
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            if (used is null)
            {
                throw new ArgumentNullException(nameof(used));
            }

            if (bindingContainerBox is null)
            {
                throw new ArgumentNullException(nameof(bindingContainerBox));
            }

            if (used.Contains(bindingContainer))
            {
                //found cycle, skip this subtree as circular
                return false;
            }
            used.Add(bindingContainer);

            if (bindingContainer.IsConditional)
            {
                return true;
            }

            if (bindingContainerBox.TryGetChildren(bindingContainer, true, out var pairs))
            {
                foreach (var pair in pairs)
                {
                    var used2 = new HashSet<IBindingContainer>(used);
                    if (CheckForItselfOrAnyChildIsConditional(
                        ref used2,
                        bindingContainerBox,
                        pair.BindingExtender.BindingContainer
                        ))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void AnalyzeForCircularDependencies(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            new CycleChecker(
                diagnosticReporter
                )
                .CheckForCycles(Box);
        }

        #endregion
    }

}
