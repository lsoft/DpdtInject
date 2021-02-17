using System;
using System.Collections.Generic;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.TypeScanner
{
    internal class TimedTypeScanner : ITypeScanner
    {
        private readonly IDiagnosticReporter _diagnosticReporter;
        private readonly ITypeScanner _typeScanner;

        public TimedTypeScanner(
            IDiagnosticReporter diagnosticReporter,
            ITypeScanner typeScanner
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (typeScanner is null)
            {
                throw new ArgumentNullException(nameof(typeScanner));
            }

            _diagnosticReporter = diagnosticReporter;
            _typeScanner = typeScanner;
        }

        public IReadOnlyList<INamedTypeSymbol> Scan(ITypeInfoProvider typeInfoProvider)
        {
            using (new DTimer(_diagnosticReporter, "Dpdt scan for types taken"))
            {
                return _typeScanner.Scan(typeInfoProvider);
            }
        }
    }
}
