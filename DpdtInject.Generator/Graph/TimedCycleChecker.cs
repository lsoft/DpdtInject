using DpdtInject.Generator.Binding;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Graph
{
    public class TimedCycleChecker : ICycleChecker
    {
        private readonly IDiagnosticReporter _diagnosticReporter;
        private readonly ICycleChecker _cycleChecker;

        public TimedCycleChecker(
            IDiagnosticReporter diagnosticReporter,
            ICycleChecker cycleChecker
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (cycleChecker is null)
            {
                throw new ArgumentNullException(nameof(cycleChecker));
            }
            _diagnosticReporter = diagnosticReporter;
            _cycleChecker = cycleChecker;
        }

        public void CheckForCycles(
            BindingExtenderBox box
            )
        {
            using (new DTimer(_diagnosticReporter, "Dpdt cycle checker time taken"))
            {
                _cycleChecker.CheckForCycles(box);
            }
        }
    }

}