using DpdtInject.Generator.Parser.Binding.Graph;
using DpdtInject.Generator.Reporter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingsContainer
    {
        private readonly List<BindingContainer> _bindingProcessors;

        public IReadOnlyList<BindingContainer> BindingProcessors => _bindingProcessors;

        public BindingsContainer(
            List<BindingContainer> bindingProcessors
            )
        {
            if (bindingProcessors is null)
            {
                throw new ArgumentNullException(nameof(bindingProcessors));
            }
            _bindingProcessors = bindingProcessors;
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
                _bindingProcessors.FindAll(bc => bc.FromTypeFullNames.Contains(bindFromTypeFullName));
        }

        public BindingContainerGroups ConvertToGroups()
        {
            return new BindingContainerGroups(_bindingProcessors);
        }

        internal void AnalyzeForCircularDependencies(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            new CycleChecker(ConvertToGroups())
                .CheckForCycles(diagnosticReporter)
                ;
        }
    }
}
