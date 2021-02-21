using DpdtInject.Generator.Reporter;
using DpdtInject.Generator.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
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
                var diagnosticReporter = new DiagnosticReporter(
                    context
                    );

                var needToStoreGeneratedSources = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
                    $"build_property.Dpdt_Generator_GeneratedSourceFolder",
                    out var generatedSourceFolder
                    );

                var generatedSourceFolderFullPath =
                    generatedSourceFolder ?? Path.GetFullPath("Dpdt.Pregenerated");

                if (needToStoreGeneratedSources)
                {
                    if (Directory.Exists(generatedSourceFolderFullPath))
                    {
                        Directory.Delete(generatedSourceFolderFullPath, true);
                    }

                    Directory.CreateDirectory(generatedSourceFolderFullPath);
                }

                var typeInfoContainer = new GeneratorTypeInfoContainer(
                    ref context,
                    generatedSourceFolderFullPath
                    );

                var internalGenerator = new DpdtInternalGenerator(
                    diagnosticReporter
                    );

                internalGenerator.Execute(
                    typeInfoContainer
                    );

                diagnosticReporter.ReportWarning(
                    "Dpdt generator successfully finished its work",
                    $"Dpdt generator successfully finished its work, {typeInfoContainer.UnitsGenerated} compilation unit(s) generated."
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
