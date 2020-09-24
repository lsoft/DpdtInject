using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Module;
using DpdtInject.Generator.Scanner;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DpdtInject.Generator
{
    public class DpdtInternalGenerator
    {
        private readonly IDiagnosticReporter _diagnosticReporter;
        private readonly string? _generatedFilePath;

        public DpdtInternalGenerator(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            _generatedFilePath = null;
            _diagnosticReporter = diagnosticReporter;
        }

        public DpdtInternalGenerator(
            IDiagnosticReporter diagnosticReporter,
            string generatedFilePath
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (string.IsNullOrEmpty(generatedFilePath))
            {
                throw new ArgumentException($"'{nameof(generatedFilePath)}' cannot be null or empty", nameof(generatedFilePath));
            }

            _diagnosticReporter = diagnosticReporter;
            _generatedFilePath = generatedFilePath;
        }
        public IReadOnlyList<ModificationDescription> Execute(
            Compilation compilation
            )
        {
            using (new DTimer(_diagnosticReporter, "Dpdt total time taken"))
            {
                var result = ExecutePrivate(
                    compilation
                );

                return result;
            }
        }

        private IReadOnlyList<ModificationDescription> ExecutePrivate(
            Compilation compilation
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

            var moduleTypes = scanner.Scan(
                compilation
                );

            foreach (var moduleType in moduleTypes)
            {
                MethodDeclarationSyntax loadMethodSyntax;
                CompilationUnitSyntax? compilationUnitSyntax;
                using (new DTimer(_diagnosticReporter, "unsorted time taken"))
                {
                    var loadMethods = moduleType.GetMembers(nameof(DpdtModule.Load));
                    if (loadMethods.Length != 1)
                    {
                        throw new Exception($"Something wrong with type {moduleType.GetFullName()}");
                    }

                    var loadMethod = loadMethods[0];

                    var loadMethodRefs = loadMethod.DeclaringSyntaxReferences;
                    if (loadMethodRefs.Length != 1)
                    {
                        throw new Exception($"Something wrong with method {loadMethod.GetFullName()} : {loadMethodRefs.Length}");
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
                    new BindExtractor(
                        compilation,
                        compilationUnitSyntax
                        )
                    );

                bindExtractor.Visit(loadMethodSyntax);

                var bindingsContainer = bindExtractor.GetBindingsContainer(
                    moduleType
                    );

                var itemGeneratorsContainer =
                    new TimedInstanceContainerGeneratorsContainer(
                        _diagnosticReporter,
                        new InstanceContainerGeneratorsContainer(
                            _diagnosticReporter,
                            compilation,
                            bindingsContainer
                            )
                        );

                itemGeneratorsContainer.Analyze(_diagnosticReporter);

                var moduleGenerator = new TimedModuleGenerator(
                    _diagnosticReporter,
                    new ModuleGenerator(
                        compilation,
                        moduleType
                        )
                    );

                var moduleSourceCode = moduleGenerator.GenerateModuleBody(
                    itemGeneratorsContainer
                    );

                ModificationDescription modificationDescription;
                using (new DTimer(_diagnosticReporter, "Dpdt beautify generated code time taken"))
                {
                    modificationDescription = new ModificationDescription(
                        moduleType,
                        moduleType.Name + Guid.NewGuid().RemoveMinuses() + "_1.cs",
                        moduleSourceCode,
                        !string.IsNullOrEmpty(_generatedFilePath)
                        );
                }

                if (!string.IsNullOrEmpty(_generatedFilePath))
                {
                    //var generatedSource = string.Join(Environment.NewLine,
                    //    modificationDescription.NewFileBody
                    //        .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                    //        .Select(l => "//" + l)
                    //    );
                    var generatedSource = modificationDescription.NewFileBody;

                    File.WriteAllText(_generatedFilePath, generatedSource);
                }

                result.Add(modificationDescription);
            }

            return result;
        }
    }
}
