using DpdtInject.Generator.Reporter;
using DpdtInject.Generator.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;
using System.IO;

namespace DpdtInject.Generator
{
    [Generator]
    public class DpdtGenerator : ISourceGenerator
    {
        public DpdtGenerator()
        {
        }
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var doBeautify = true;
                var beautifyExists = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
                    $"build_property.Dpdt_Generator_Beautify",
                    out var beautify
                    );
                if (beautifyExists)
                {
                    if (bool.TryParse(beautify?.ToLower() ?? "false", out var parsed))
                    {
                        doBeautify = parsed;
                    }
                }

                var diagnosticReporter = new DiagnosticReporter(
                    context
                    );

                var sw = Stopwatch.StartNew();

                var typeInfoContainer = new GeneratorTypeInfoContainer(
                    ref context,
                    doBeautify
                    );

                var internalGenerator = new DpdtInternalGenerator(
                    diagnosticReporter
                    );

                internalGenerator.Execute(
                    typeInfoContainer
                    );

                diagnosticReporter.ReportWarning(
                    "Dpdt generator successfully finished its work",
                    $"Dpdt generator successfully finished its work, {typeInfoContainer.UnitsGenerated} compilation unit(s) generated, taken {sw.Elapsed}."
                    );
            }
            catch (Exception excp)
            {
                Logging.LogGen(excp);

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "DPDTINJECT100",
                            title: "Couldn't generate a binding boilerplate code",
                            messageFormat: "Couldn't generate a binding boilerplate code '{0}' {1}",
                            category: "DpDtInject",
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true
                            ),
                        Location.None,
                        excp.Message,
                        excp.StackTrace
                        )
                    );
            }
        }

    }
}
