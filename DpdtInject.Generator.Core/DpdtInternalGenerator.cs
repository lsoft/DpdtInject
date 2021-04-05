using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Generator.Core.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Core.BindExtractor.Parsed;
using DpdtInject.Generator.Core.TypeScanner;
using DpdtInject.Generator.Core.BindExtractor.Parsed.Factory;
using DpdtInject.Generator.Core.Binding.Xml;
using DpdtInject.Generator.Core.Meta;
using DpdtInject.Injector.Helper;

namespace DpdtInject.Generator.Core
{
    public class DpdtInternalGenerator
    {
        public const string DpdtXmlArtifactFile = "dpdt_artifact.xml";

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

            var clusterMethodBindings = DoExtraction(
                typeInfoContainer
                );

            DoGenerateXml(
                typeInfoContainer,
                clusterMethodBindings
                );

            DoGenerateBindingSourceCode(typeInfoContainer, clusterMethodBindings);
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
                        new BindClauseExtractor(
                            semanticModelDecorator,
                            new DetermineBindExpressionFactory(
                                new ExplicitBindExpressionFactory(
                                    typeInfoContainer,
                                    semanticModelDecorator,
                                    new ConstructorArgumentFromSyntaxExtractor(
                                        semanticModelDecorator
                                        ),
                                    new ConstructorArgumentDetector(
                                        new BindConstructorChooser()
                                        )
                                    ),
                                new ConventionalBindExpressionFactory(
                                    typeInfoContainer,
                                    new ConstructorArgumentFromSyntaxExtractor(
                                        semanticModelDecorator
                                        ),
                                    new ConstructorArgumentDetector(
                                        new BindConstructorChooser()
                                        ),
                                    _diagnosticReporter
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


        private void DoGenerateXml(
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



            var clusterXmls = new List<ClusterBindContainerXml>();
            foreach (var clusterMethodBinding in clusterMethodBindings)
            {
                var clusterType = clusterMethodBinding.ClusterType;
                var clusterTypeXml = clusterType.ToXml();

                var methodBindXmls = new List<MethodBindContainerXml>();
                foreach (var methodBinding in clusterMethodBinding.MethodBindings)
                {
                    var methodDeclaration = methodBinding.Item1;

                    var methodDeclarationXml = new MethodDeclarationInfoXml(
                        methodDeclaration.ToXml(),
                        methodDeclaration.Identifier.Text
                        );

                    var methodBindXml = new MethodBindContainerXml(
                        clusterTypeXml,
                        methodDeclarationXml,
                        methodBinding.Item2.ConvertAll(
                            bc => new BindingXml(
                                bc.Identifier.ToString(),
                                bc.TargetRepresentation,
                                bc.BindFromTypes.ConvertAll(s => s.ToXml()).ToArray(),
                                bc.BindToType.ToXml(),
                                bc.Scope.ToString(),
                                (int)bc.Scope,
                                bc.IsConditional,
                                bc.IsConventional,
                                bc.ExpressionNode.ToXml()
                                )
                            ).ToArray()
                        );
                    methodBindXmls.Add(methodBindXml);
                }

                var clusterXml = new ClusterBindContainerXml(
                    clusterTypeXml,
                    methodBindXmls.ToArray()
                    );
                clusterXmls.Add(clusterXml);
            }

            var solutionXml = new SolutionBindContainerXml(
                clusterXmls.ToArray()
                );

            var meta = new BuiltinMeta();
            meta.Store(
                typeInfoContainer,
                solutionXml
                );
        }

        private void DoGenerateBindingSourceCode(
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
