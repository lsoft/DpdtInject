using DpdtInject.Generator.BindExtractor;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.BindExtractor.Parsed;
using DpdtInject.Generator.TypeScanner;

namespace DpdtInject.Generator
{
    public class DpdtInternalGenerator
    {
        private readonly IDiagnosticReporter _diagnosticReporter;
        private readonly bool _doBeautify;

        public DpdtInternalGenerator(
            IDiagnosticReporter diagnosticReporter,
            bool doBeautify
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            _diagnosticReporter = diagnosticReporter;
            _doBeautify = doBeautify;
        }

        public void Execute(
            ITypeInfoContainer typeInfoContainer
            )
        {
            if (typeInfoContainer is null)
            {
                throw new ArgumentNullException(nameof(typeInfoContainer));
            }

            var cmb = DoExtraction(
                typeInfoContainer
                );

            DoGeneration(typeInfoContainer, cmb);
        }


        public IReadOnlyList<ClusterMethodBindings> DoExtraction(
            ITypeInfoContainer typeInfoContainer
            )
        {
            if (typeInfoContainer is null)
            {
                throw new ArgumentNullException(nameof(typeInfoContainer));
            }

            var scanner = new DefaultTypeScanner(
                );

            var clusterTypes = scanner.ScanForClusterTypes(
                typeInfoContainer
                );

            var stepResults = new List<ClusterMethodBindings>();
            for (var clusterTypeIndex = 0; clusterTypeIndex < clusterTypes.Count; clusterTypeIndex++)
            {
                var clusterType = clusterTypes[clusterTypeIndex];

                clusterType.ScanForRequiredSyntaxes(
                    out List<MethodDeclarationSyntax> bindMethodSyntaxes,
                    out List<CompilationUnitSyntax> compilationUnitSyntaxes
                    );

                if (compilationUnitSyntaxes.Count == 0)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Unknown problem to access to compilation unit syntax"
                        );
                }

                var semanticModels = new List<SemanticModel>();
                var moduleUnitUsings = new List<UsingDirectiveSyntax>();
                foreach (var compilationUnitSyntax in compilationUnitSyntaxes)
                {
                    var moduleUnitUsing = compilationUnitSyntax
                        .DescendantNodes()
                        .OfType<UsingDirectiveSyntax>()
                        .ToList();
                    moduleUnitUsings.AddRange(moduleUnitUsing);

                    var semanticModel = typeInfoContainer.GetSemanticModel(compilationUnitSyntax.SyntaxTree);
                    semanticModels.Add(semanticModel);
                }

                var semanticModelDecorator = new SemanticModelDecorator(
                    semanticModels
                    );

                var stepResult = new ClusterMethodBindings(
                    clusterType,
                    moduleUnitUsings
                    );

                foreach (var bindMethodSyntax in bindMethodSyntaxes)
                {
                    var bindExtractor =
                        new DefaultBindExtractor(
                            semanticModelDecorator,
                            new ParsedBindExpressionFactory(
                                typeInfoContainer,
                                semanticModelDecorator,
                                new ConstructorArgumentFromSyntaxExtractor(
                                    semanticModelDecorator
                                    ),
                                new ConstructorArgumentDetector(
                                    new BindConstructorChooser()
                                    )
                                )
                            );

                    bindExtractor.Visit(bindMethodSyntax);

                    stepResult.AddMethodBindings(
                        bindMethodSyntax,
                        bindExtractor.BindingContainers
                        );
                }

                stepResults.Add(stepResult);
            }

            return stepResults;
        }

        private void DoGeneration(
            ITypeInfoContainer typeInfoContainer,
            IReadOnlyList<ClusterMethodBindings> clusterMethodBindings
            )
        {
            if (typeInfoContainer is null)
            {
                throw new ArgumentNullException(nameof(typeInfoContainer));
            }

            if (clusterMethodBindings is null)
            {
                throw new ArgumentNullException(nameof(clusterMethodBindings));
            }

            var srIndex = 0;
            foreach (var clusterMethodBinding in clusterMethodBindings)
            {
                var clusterBindings = clusterMethodBinding.GetClusterBindings(
                    );

                clusterBindings.BuildFlags(
                    );

                clusterBindings.Analyze(
                    _diagnosticReporter
                    );

                //build the cluster:
                var clusterProducer = new ClusterProducer(
                    typeInfoContainer,
                    clusterBindings,
                    _doBeautify
                    );

                var moduleSourceCode = clusterProducer.Produce(
                    clusterMethodBinding.ModuleUnitUsings
                    );

                var modificationDescription = new ModificationDescription(
                    clusterMethodBinding.ClusterType,
                    $"{clusterMethodBinding.ClusterType.Name}.Pregenerated{srIndex}.cs",
                    moduleSourceCode
                    );

                typeInfoContainer.AddSources(new[] { modificationDescription });

                srIndex++;
            }
        }
    }
}
