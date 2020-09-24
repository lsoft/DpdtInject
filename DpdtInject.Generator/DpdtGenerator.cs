using DpdtInject.Generator.Reporter;
using DpdtInject.Injector;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.IO;
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

                var internalGenerator = new DpdtInternalGenerator(
                    diagnosticReporter
                    );

                var needToStoreGeneratedSources = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
                    $"build_property.Dpdt_Generator_GeneratedSourceFolder",
                    out var generatedSourceFolder
                    );

                var generatedSourceFolderFullPath =
                    Path.GetFullPath(
                        generatedSourceFolder ?? "Dpdt.Pregenerated"
                        );

                if (needToStoreGeneratedSources)
                {
                    if (Directory.Exists(generatedSourceFolderFullPath))
                    {
                        Directory.Delete(generatedSourceFolderFullPath, true);
                    }

                    Directory.CreateDirectory(generatedSourceFolderFullPath);
                }

                var unitsGenerated = 0;
                foreach (var modificationDescription in internalGenerator.Execute(context.Compilation))
                {
                    if (needToStoreGeneratedSources)
                    {
                        File.WriteAllText(
                            Path.Combine(generatedSourceFolderFullPath, modificationDescription.NewFileName),
                            modificationDescription.NewFileBody
                            );
                    }

                    context.AddSource(
                        modificationDescription.NewFileName,
                        SourceText.From(modificationDescription.NewFileBody, Encoding.UTF8)
                        );

                    unitsGenerated++;
                }

                diagnosticReporter.ReportWarning(
                    "Dpdt generator successfully finished its work",
                    $"Dpdt generator successfully finished its work, {unitsGenerated} compilation unit(s) generated."
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
