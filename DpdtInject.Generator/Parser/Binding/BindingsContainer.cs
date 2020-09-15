using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding.Graph;
using DpdtInject.Generator.Reporter;
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
        private readonly List<BindingContainer> _bindingContainers;

        public IReadOnlyList<BindingContainer> BindingContainers => _bindingContainers;

        public BindingContainerGroups Groups
        {
            get;
        }

        public BindingsContainer(
            List<BindingContainer> bindingContainers
            )
        {
            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            _bindingContainers = bindingContainers;

            Groups = new BindingContainerGroups(_bindingContainers);
        }

        public IReadOnlyList<BindingContainer> GetBindWith(
            string bindFromTypeFullName
            )
        {
            if (bindFromTypeFullName is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypeFullName));
            }

            return
                _bindingContainers.FindAll(bc => bc.FromTypeFullNames.Contains(bindFromTypeFullName));
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
        internal bool CheckForAtLeastOneParentIsConditional(
            IDiagnosticReporter diagnosticReporter,
            BindingContainer bindingContainer
            )
        {
            return
                CheckForAtLeastOneParentIsConditionalPrivate(
                    diagnosticReporter,
                    bindingContainer,
                    bindingContainer,
                    new HashSet<BindingContainer>()
                    );
        }
        private bool CheckForAtLeastOneParentIsConditionalPrivate(
            IDiagnosticReporter diagnosticReporter,
            BindingContainer rootBindingContainer,
            BindingContainer bindingContainer,
            HashSet<BindingContainer> processed
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
                    $"Searching for undeterministic resolution path for [{rootBindingContainer.BindToType.GetFullName()}] has been skipped, because of circular dependency found.",
                    $""
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
            BindingContainer bindingContainer
            )
        {
            throw new NotImplementedException("wrote, but not checked!");

            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            foreach(var ca in bindingContainer.ConstructorArguments.Where(j => !j.DefineInBindNode))
            {
                if(!Groups.BindGroups.TryGetValue(ca.Type!, out var children))
                {
                    throw new DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, $"Found unknown binding [{ca.Type.GetFullName()}] from constructor of [{bindingContainer.BindToType.GetFullName()}]", ca.Type.Name);
                }

                foreach(var child in children)
                {
                    if(CheckForAtLeastOneChildIsConditional(
                        child
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
