using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Module;
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

        public IEnumerable<ModificationDescription> Execute(
            Compilation compilation
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            var allTypes = compilation.GlobalNamespace.GetAllTypes().ToList();
            var moduleTypes = allTypes
                //.Where(t => !t.ContainingAssembly.GetAttributes().Any(a => a.AttributeClass.GetFullName() == typeof(DpDtSkipAssembly).FullName))
                .Where(t => t.BaseType != null)
                .Where(t => t.BaseType!.GetFullName() == typeof(DpdtModule).FullName)
                //.Where(t => t.GetFullName() == "DpdtInject.TestConsole.xxx")
                .ToList()
                ;

            //yield break;

            foreach (var moduleType in moduleTypes)
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

                //var loadMethodRef = moduleType.GetOnlyMethod(nameof(DpdtModule.Load));
                var loadMethodSyntax = (MethodDeclarationSyntax)loadMethodRef.GetSyntax();

                var compilationUnitSyntax = loadMethodSyntax.Root<CompilationUnitSyntax>();
                if (compilationUnitSyntax == null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Unknown problem to access to compilation unit syntax"
                        );
                }

                var bindExtractor = new BindExtractor(
                    compilation,
                    compilationUnitSyntax
                    );

                bindExtractor.Visit(loadMethodSyntax);

                //var rewrittenMethodSyntax = (MethodDeclarationSyntax)bindRewriter.Visit(loadMethodSyntax);
                //if (rewrittenMethodSyntax == null)
                //{
                //    throw new DpdtException(
                //        DpdpExceptionTypeEnum.InternalError,
                //        $"Unknown problem to access to rewritten unit syntax"
                //        );
                //}
                //if (rewrittenMethodSyntax.Body == null)
                //{
                //    throw new DpdtException(
                //        DpdpExceptionTypeEnum.InternalError,
                //        $"Unknown problem to access to rewritten unit syntax body"
                //        );
                //}


                //                var modifiedMethodBody = SyntaxFactory.ParseMemberDeclaration(
                //                    $@"public override void Apply(
                //{nameof(IInstancesContainer)} containers
                //)" + rewrittenMethodSyntax.Body.GetText().ToString()
                //                    );

                //                var customCompilationUnitUsings = string.Empty;
                //                var cus = loadMethodSyntax.Root<CompilationUnitSyntax>();
                //                if (cus != null)
                //                {
                //                    customCompilationUnitUsings = string.Join(
                //                        Environment.NewLine,
                //                        cus.Usings.ToFullString()
                //                        );
                //                }

                //                var customNamespaceUsings = string.Empty;
                //                var nus = loadMethodSyntax.Root<NamespaceDeclarationSyntax>();
                //                if (nus != null)
                //                {
                //                    customNamespaceUsings = string.Join(
                //                        Environment.NewLine,
                //                        nus.Usings.ToFullString()
                //                        );
                //                }

                //                var containerClasses = string.Join(
                //                    Environment.NewLine,
                //                    bindRewriter.ContainerClassDeclarations
                //                    );

                //var bindingProcessor = new BindingProcessor(
                //    new List<string> { "IA" },
                //    "A",
                //    "DpdtInject.Tests.Scope.Singleton.A"
                //    );

                //var bindingProcessors = new List<BindingProcessor>()
                //{
                //    bindingProcessor
                //};

                var bindingsContainer = bindExtractor.GetBindingsContainer();
                var itemGeneratorsContainer = new InstanceContainerGeneratorsContainer(
                    _diagnosticReporter,
                    bindingsContainer
                    );

                var modulePartGenerator = new ModuleGenerator(
                    _diagnosticReporter,
                    moduleType
                    );

                var modificationDescription = new ModificationDescription(
                    moduleType,
                    moduleType.Name + Guid.NewGuid().ToString().ConvertMinusToGround() + "_1.cs",
                    modulePartGenerator.GetClassBody(itemGeneratorsContainer)
                    );

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

                yield return modificationDescription;
            }

        }
    }
}
