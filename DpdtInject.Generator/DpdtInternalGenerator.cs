using DpdtInject.Generator.BindExtractor;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer;
using DpdtInject.Generator.Scanner;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;

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

        public IReadOnlyList<ModificationDescription> Execute(
            Compilation compilation,
            Func<int, string>? generatedFilePathFunc
            )
        {
            using (new DTimer(_diagnosticReporter, "Dpdt total time taken"))
            {
                var result = ExecutePrivate(
                    compilation,
                    generatedFilePathFunc
                );

                return result;
            }
        }

        private IReadOnlyList<ModificationDescription> ExecutePrivate(
            Compilation compilation,
            Func<int, string>? generatedFilePathFunc
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            var result = new List<ModificationDescription>();

            var scanner = new TimedTypeScanner(
                _diagnosticReporter,
                new DefaultTypeScanner(
                    )
                );

            var clusterTypes = scanner.Scan(
                compilation
                );

            for(var clusterTypeIndex = 0; clusterTypeIndex < clusterTypes.Count; clusterTypeIndex++)
            {
                var clusterType = clusterTypes[clusterTypeIndex];

                MethodDeclarationSyntax loadMethodSyntax;
                CompilationUnitSyntax? compilationUnitSyntax;
                using (new DTimer(_diagnosticReporter, "unsorted time taken"))
                {
                    var loadMethods = clusterType.GetMembers(nameof(DefaultCluster.Load));
                    if (loadMethods.Length != 1)
                    {
                        throw new Exception($"Something wrong with type {clusterType.ToDisplayString()}");
                    }

                    var loadMethod = loadMethods[0];

                    var loadMethodRefs = loadMethod.DeclaringSyntaxReferences;
                    if (loadMethodRefs.Length != 1)
                    {
                        throw new Exception($"Something wrong with method {loadMethod.ToDisplayString()} : {loadMethodRefs.Length}");
                    }

                    var loadMethodRef = loadMethodRefs[0];

                    loadMethodSyntax = (MethodDeclarationSyntax)loadMethodRef.GetSyntax();
                    compilationUnitSyntax = loadMethodSyntax.Root<CompilationUnitSyntax>();
                }

                if (compilationUnitSyntax == null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Unknown problem to access to compilation unit syntax"
                        );
                }

                var bindExtractor = new TimedBindExtractor(
                    _diagnosticReporter,
                    new DefaultBindExtractor(
                        compilation,
                        compilationUnitSyntax
                        )
                    );

                bindExtractor.Visit(loadMethodSyntax);

                var clusterBindings = bindExtractor.GetClusterBindings(
                    clusterType
                    );

                clusterBindings.BuildFlags(
                    );

                clusterBindings.Analyze(
                    _diagnosticReporter
                    );

                var clusterProducer = new ClusterProducer(
                    compilation,
                    clusterBindings
                    );

                var moduleSourceCode = clusterProducer.Produce();

                ModificationDescription modificationDescription;
                using (new DTimer(_diagnosticReporter, "Dpdt beautify generated code time taken"))
                {
                    modificationDescription = new ModificationDescription(
                        clusterType,
                        clusterType.Name + Guid.NewGuid().RemoveMinuses() + "_1.cs",
                        moduleSourceCode,
                        generatedFilePathFunc is not null
                        );
                }

                if (generatedFilePathFunc is not null)
                {
                    //var generatedSource = string.Join(Environment.NewLine,
                    //    modificationDescription.NewFileBody
                    //        .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                    //        .Select(l => "//" + l)
                    //    );
                    var generatedSource = modificationDescription.NewFileBody;

                    var generatedFilePath = generatedFilePathFunc(clusterTypeIndex);

                    File.WriteAllText(generatedFilePath, generatedSource);
                }

                result.Add(modificationDescription);
            }

            return result;
        }
    }
}
