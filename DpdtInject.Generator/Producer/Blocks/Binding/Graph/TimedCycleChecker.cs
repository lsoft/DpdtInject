﻿using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using System;

namespace DpdtInject.Generator.Producer.Blocks.Binding.Graph
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

        public void CheckForCycles(InstanceContainerGeneratorTreeJoint joint)
        {
            using (new DTimer(_diagnosticReporter, "Dpdt cycle checker time taken"))
            {
                _cycleChecker.CheckForCycles(joint);
            }
        }
    }

}