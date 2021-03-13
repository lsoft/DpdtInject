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
                        foreach (var file in Directory.GetFiles(generatedSourceFolderFullPath))
                        {
                            File.Delete(file);
                        }
                        foreach (var directory in Directory.GetDirectories(generatedSourceFolderFullPath))
                        {
                            Directory.Delete(directory);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(generatedSourceFolderFullPath);
                    }
                }

                var sw = Stopwatch.StartNew();

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
                    $"Dpdt generator successfully finished its work, {typeInfoContainer.UnitsGenerated} compilation unit(s) generated, taken {sw.Elapsed}."
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
