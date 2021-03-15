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

                if (needToStoreGeneratedSources)
                {
                    try
                    {
                        if (Directory.Exists(generatedSourceFolder))
                        {
                            foreach (var file in Directory.GetFiles(generatedSourceFolder!))
                            {
                                File.Delete(file);
                            }
                            foreach (var directory in Directory.GetDirectories(generatedSourceFolder!))
                            {
                                Directory.Delete(directory);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(generatedSourceFolder!);
                        }
                    }
                    catch (Exception excp)
                    {
                        Logging.LogGen($"Working with '{generatedSourceFolder}' fails due to:");
                        Logging.LogGen(excp);
                    }
                }

                var sw = Stopwatch.StartNew();

                var typeInfoContainer = new GeneratorTypeInfoContainer(
                    ref context,
                    generatedSourceFolder
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
