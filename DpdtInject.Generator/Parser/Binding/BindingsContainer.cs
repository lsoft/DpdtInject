using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding.Graph;
using DpdtInject.Generator.Reporter;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingsContainer
    {
        private readonly List<IBindingContainer> _bindingContainers;

        public IReadOnlyList<IBindingContainer> BindingContainers => _bindingContainers;

        public BindingContainerGroups Groups
        {
            get;
        }

        public BindingsContainer(
            List<IBindingContainer> bindingContainers
            )
        {
            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            _bindingContainers = bindingContainers;

            Groups = new BindingContainerGroups(_bindingContainers);
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

        internal void AnalyzeForSingletonTakesTransient(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            foreach (var bindingContainer in _bindingContainers)
            {
                AnalyzeForSingletonTakesTransientPrivate(
                    diagnosticReporter,
                    bindingContainer,
                    new HashSet<IBindingContainer>()
                    );
            }
        }

        private void AnalyzeForSingletonTakesTransientPrivate(
            IDiagnosticReporter diagnosticReporter,
            IBindingContainer parent,
            HashSet<IBindingContainer> processed
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
                    $"Searching for singleton-transient relationship has been skipped, because of circular dependency found with {parent.TargetRepresentation}."
                    );

                return;
            }

            processed.Add(parent);

            foreach (var ca in parent.ConstructorArguments.Where(j => !j.DefineInBindNode))
            {
                if (!Groups.BindGroups.TryGetValue(ca.Type!, out var children))
                {
                    throw new DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, $"Found unknown binding [{ca.Type!.GetFullName()}] from constructor of [{parent.TargetRepresentation}]", ca.Type.Name);
                }

                foreach (var child in children)
                {
                    if(parent.Scope.In(BindScopeEnum.Singleton))
                    {
                        if(child.Scope.In(BindScopeEnum.Transient))
                        {
                            diagnosticReporter.ReportWarning(
                                $"Singleton-transient relationship has been found.",
                                $"Searching for singleton-transient relationship has been found: singleton parent [{parent.TargetRepresentation}] takes transient child [{child.TargetRepresentation}]."
                                );
                        }
                    }

                    AnalyzeForSingletonTakesTransientPrivate(
                        diagnosticReporter,
                        child,
                        new HashSet<IBindingContainer>(processed)
                        );
                }
            }
        }


        internal bool CheckForAtLeastOneParentIsConditional(
            IDiagnosticReporter diagnosticReporter,
            IBindingContainer bindingContainer
            )
        {
            return
                CheckForAtLeastOneParentIsConditionalPrivate(
                    diagnosticReporter,
                    bindingContainer,
                    bindingContainer,
                    new HashSet<IBindingContainer>()
                    );
        }

        private bool CheckForAtLeastOneParentIsConditionalPrivate(
            IDiagnosticReporter diagnosticReporter,
            IBindingContainer rootBindingContainer,
            IBindingContainer bindingContainer,
            HashSet<IBindingContainer> processed
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            if(processed.Contains(bindingContainer))
            {
                //circular dependency found
                //do not check this binding because of it's invalid a priori
                diagnosticReporter.ReportWarning(
                    $"Searching for undeterministic resolution path has been skipped",
                    $"Searching for undeterministic resolution path for [{rootBindingContainer.TargetRepresentation}] has been skipped, because of circular dependency found."
                    );
                return false;
            }

            processed.Add(bindingContainer);

            foreach (var bindFromType in bindingContainer.BindFromTypes)
            {
                if (!Groups.NotBindParentGroups.TryGetValue(bindFromType, out var parents))
                {
                    //no parent for bindFromType exists
                    //so no conditional parent exists
                    //it's completely ok
                    return false;
                }

                foreach (var parent in parents)
                {
                    if(parent.IsConditional)
                    {
                        return true;
                    }

                    if (CheckForAtLeastOneParentIsConditionalPrivate(
                        diagnosticReporter,
                        rootBindingContainer,
                        parent,
                        processed
                        ))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal bool CheckForAtLeastOneChildIsConditional(
            IDiagnosticReporter diagnosticReporter,
            IBindingContainer bindingContainer
            )
        {
            return
                CheckForAtLeastOneChildIsConditionalPrivate(
                    diagnosticReporter,
                    bindingContainer,
                    bindingContainer,
                    new HashSet<IBindingContainer>()
                    );

        }

        private bool CheckForAtLeastOneChildIsConditionalPrivate(
            IDiagnosticReporter diagnosticReporter,
            IBindingContainer rootBindingContainer,
            IBindingContainer bindingContainer,
            HashSet<IBindingContainer> processed
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            if (processed.Contains(bindingContainer))
            {
                //circular dependency found
                //do not check this binding because of it's invalid a priori
                diagnosticReporter.ReportWarning(
                    $"Searching for undeterministic resolution path (up) has been skipped.",
                    $"Searching for undeterministic resolution path (up) for [{rootBindingContainer.TargetRepresentation}] has been skipped, because of circular dependency found."
                    );
                return false;
            }

            processed.Add(bindingContainer);

            foreach (var ca in bindingContainer.ConstructorArguments.Where(j => !j.DefineInBindNode))
            {
                if (!Groups.BindGroups.TryGetValue(ca.Type!, out var children))
                {
                    throw new DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, $"Found unknown binding [{ca.Type!.GetFullName()}] from constructor of [{bindingContainer.TargetRepresentation}]", ca.Type.Name);
                }

                foreach (var child in children)
                {
                    if (child.IsConditional)
                    {
                        return true;
                    }

                    if (CheckForAtLeastOneChildIsConditionalPrivate(
                        diagnosticReporter,
                        rootBindingContainer,
                        child,
                        processed
                        ))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
