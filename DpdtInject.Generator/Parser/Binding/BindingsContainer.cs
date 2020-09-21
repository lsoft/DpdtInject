using DpdtInject.Generator.Helpers;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingsContainer
    {
        private readonly List<IBindingContainer> _bindingContainers;
        private readonly BindingContainerGroups _groups;

        public IReadOnlyList<IBindingContainer> BindingContainers => _bindingContainers;

        public BindingsContainer(
            List<IBindingContainer> bindingContainers
            )
        {
            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            _bindingContainers = bindingContainers;

            _groups = new BindingContainerGroups(
                _bindingContainers
                );

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
                if (!_groups.NotBindParentGroups.TryGetValue(bindFromType, out var parents))
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
                        new HashSet<IBindingContainer>(processed)
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
                if (ca.Type is null)
                {
                    throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, $"ca.Type is null somehow");
                }

                if (!_groups.TryGetRegisteredBindingContainers(ca.Type!, true, out var children))
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
                        new HashSet<IBindingContainer>(processed)
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
