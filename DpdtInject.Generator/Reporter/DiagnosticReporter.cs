using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Reporter
{
    public class DiagnosticReporter : IDiagnosticReporter
    {
        private readonly GeneratorExecutionContext _context;

        public DiagnosticReporter(
            GeneratorExecutionContext context
            )
        {
            _context = context;
        }

        public void ReportError(string title, string message)
        {
            _context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "DPDTINJECT200",
                        title: title,
                        messageFormat: message,
                        category: "DpDtInject",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true
                        ),
                    Location.None
                    )
                );
        }

        public void ReportWarning(string title, string message)
        {
            _context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "DPDTINJECT100",
                        title: title,
                        messageFormat: message,
                        category: "DpDtInject",
                        DiagnosticSeverity.Warning,
                        isEnabledByDefault: true
                        ),
                    Location.None
                    )
                );
        }
    }

}
