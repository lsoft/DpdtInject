using DpdtInject.Generator.BindExtractor;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer;
using DpdtInject.Generator.Producer.Factory;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DpdtInject.Generator.TypeScanner;

namespace DpdtInject.Generator
{
    public class DpdtInternalGenerator
    {
        private readonly IDiagnosticReporter _diagnosticReporter;

        public DpdtInternalGenerator(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            _diagnosticReporter = diagnosticReporter;
        }

        public void Execute(
            ITypeInfoContainer typeInfoContainer
            )
        {
            using (new DTimer(_diagnosticReporter, "Dpdt total time taken"))
            {
                ExecutePrivate(
                    typeInfoContainer
                );
            }
        }

        private void ExecutePrivate(
            ITypeInfoContainer typeInfoContainer
            )
        {
            if (typeInfoContainer is null)
            {
                throw new ArgumentNullException(nameof(typeInfoContainer));
            }

            var scanner = new TimedTypeScanner(
                _diagnosticReporter,
                new DefaultTypeScanner(
                    )
                );

            var clusterTypes = scanner.Scan(
                typeInfoContainer
                );

            for(var clusterTypeIndex = 0; clusterTypeIndex < clusterTypes.Count; clusterTypeIndex++)
            {
                var clusterType = clusterTypes[clusterTypeIndex];

                List<MethodDeclarationSyntax> bindMethodSyntaxes = new();
                List<CompilationUnitSyntax> compilationUnitSyntaxes = new();
                using (new DTimer(_diagnosticReporter, "unsorted time taken"))
                {
                    var bindMethods = (
                        from member in clusterType.GetMembers()
                        where member is IMethodSymbol
                        let method = member as IMethodSymbol
                        where method.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == typeof(DpdtBindingMethodAttribute).FullName)
                        select method
                        ).ToArray();

                    if (bindMethods.Length == 0)
                    {
                        throw new Exception($"Something wrong with type {clusterType.ToDisplayString()} : no bind methods found. Please add at least one bind method or remove this class.");
                    }

                    foreach (var bindMethod in bindMethods)
                    {
                        var bindMethodRefs = bindMethod.DeclaringSyntaxReferences;

                        if (bindMethodRefs.Length != 1)
                        {
                            throw new Exception($"Something wrong with method {bindMethod.ToDisplayString()} : refs to bind method = {bindMethodRefs.Length}, should only one.");
                        }

                        var bindMethodRef = bindMethodRefs[0];

                        var bindMethodSyntax = (MethodDeclarationSyntax) bindMethodRef.GetSyntax();
                        bindMethodSyntaxes.Add(bindMethodSyntax);

                        var compilationUnitSyntax = bindMethodSyntax.Root<CompilationUnitSyntax>();
                        if (compilationUnitSyntax is not null)
                        {
                            //compilationUnitSyntax can repeat
                            if (compilationUnitSyntaxes.All(cus => cus.ToString() != compilationUnitSyntax.ToString()))
                            {
                                compilationUnitSyntaxes.Add(compilationUnitSyntax);
                            }
                        }
                    }
                }

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

                var bindExtractor = new TimedBindExtractor(
                    _diagnosticReporter,
                    new DefaultBindExtractor(
                        typeInfoContainer,
                        semanticModelDecorator,
                        new ConstructorArgumentFromSyntaxExtractor(typeInfoContainer, semanticModelDecorator),
                        new ConstructorArgumentDetector(
                            new BindConstructorChooser()
                            )
                        )
                    );

                foreach (var bindMethodSyntax in bindMethodSyntaxes)
                {
                    bindExtractor.Visit(bindMethodSyntax);
                }

                var clusterBindings = bindExtractor.GetClusterBindings(
                    clusterType
                    );

                clusterBindings.BuildFlags(
                    );

                clusterBindings.Analyze(
                    _diagnosticReporter
                    );


                //build the cluster:
                var clusterProducer = new ClusterProducer(
                    typeInfoContainer,
                    clusterBindings
                    );

                var moduleSourceCode = clusterProducer.Produce(
                    moduleUnitUsings
                    );

                ModificationDescription modificationDescription;
                using (new DTimer(_diagnosticReporter, "Dpdt cluster beautify generated code time taken"))
                {
                    modificationDescription = new ModificationDescription(
                        clusterType,
                        $"{clusterType.Name}.Pregenerated{clusterTypeIndex}.cs",
                        moduleSourceCode
                        );
                }

                typeInfoContainer.AddSources(new[] { modificationDescription });
            }
        }
    }
}
