using DpdtInject.Generator.Reporter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DpdtInject.Generator
{
    [Generator]
    public class DpdtGenerator : ISourceGenerator
    {
        public DpdtGenerator()
        {
        }

        public void Initialize(InitializationContext context)
        {
        }

        public void Execute(SourceGeneratorContext context)
        {
            try
            {
                var internalGenerator = new DpdtInternalGenerator(
                    new DiagnosticReporter(
                        context
                        )
                    );

                var unitsGenerated = 0;
                foreach (var modificationDescription in internalGenerator.Execute(context.Compilation))
                {
                    context.AddSource(
                        modificationDescription.NewFileName,
                        SourceText.From(modificationDescription.NewFileBody, Encoding.UTF8)
                        );

                    unitsGenerated++;
                }

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "DPDTINJECT001",
                            title: "Dpdt generator successfully finished its work",
                            messageFormat: "Dpdt generator successfully finished its work, {0} compilation unit(s) generated. [it's an info message, not a real warning; swithing from 'warning' to 'message' results in no message are shown in 'Error List' window, don't know why]",
                            category: "DpDtInject",
                            DiagnosticSeverity.Warning,
                            isEnabledByDefault: true
                            ),
                        Location.None,
                        unitsGenerated
                        )
                    );
            }
            catch (Exception excp)
            {
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
