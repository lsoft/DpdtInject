using DpdtInject.Generator.Core;
using DpdtInject.Generator.Core.Reporter;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            context.RegisterForSyntaxNotifications(() => new ClusterCandidateSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                if (!(context.SyntaxReceiver is ClusterCandidateSyntaxReceiver receiver))
                    return;

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
                    receiver.CandidateClasses
                    );

                var internalGenerator = new DpdtInternalGenerator(
                    diagnosticReporter,
                    doBeautify
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

    /// <summary>
    /// Сoarse filter for Dpdt clusters
    /// </summary>
    public class ClusterCandidateSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // any field with at least one attribute is a candidate for property generation
            if (syntaxNode is MethodDeclarationSyntax mds
                && mds.AttributeLists.Count > 0)
            {
                var cds = GetParentClass(mds);
                if (cds != null)
                {
                    if (cds.BaseList != null && cds.BaseList.Types.Count > 0)
                    {
                        if(cds.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
                        {
                            CandidateClasses.Add(cds);
                        }
                    }
                }
            }
        }

        private ClassDeclarationSyntax? GetParentClass(MethodDeclarationSyntax mds)
        {
            SyntaxNode? current = mds;
            while (current != null)
            {
                if (current is ClassDeclarationSyntax cds)
                {
                    return cds;
                }

                current = current.Parent;
            }

            return null;
        }
    }
}
