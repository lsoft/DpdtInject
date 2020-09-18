using DpdtInject.Generator.Reporter;
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
                var internalGenerator = new DpdtInternalGenerator(
                    new DiagnosticReporter(
                        context
                        )
                    );

                //if(context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.Dpdt_Generator_GeneratedSourceFolder", out var generatedSourceFolder))
                //    File.AppendAllText("c:\\temp\\__sg.txt", ": " + generatedSourceFolder + Environment.NewLine);
                //else
                //    File.AppendAllText("c:\\temp\\__sg.txt", "no" + Environment.NewLine);

                //File.AppendAllText("c:\\temp\\__sg.txt", "cd: " + Directory.GetCurrentDirectory());

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
                    if(Directory.Exists(generatedSourceFolderFullPath))
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
