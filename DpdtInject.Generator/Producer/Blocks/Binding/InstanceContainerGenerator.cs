using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
using DpdtInject.Generator.Properties;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Binding
{

    public class InstanceContainerGenerator
    {
        public BindingContainer BindingContainer
        {
            get;
        }

        public IReadOnlyList<ITypeSymbol> BindFromTypes => BindingContainer.BindFromTypes;

        public string ClassName
        {
            get;
        }

        public IReadOnlyList<string> Usings
        {
            get;
        }
        public bool AtLeastOneParentIsConditional
        {
            get;
        }

        public string DisposeClause => $"{ClassName}.{nameof(SingletonInstanceContainer.DoDisposeIfApplicable)}()";

        public string GetInstanceClause => $"{ClassName}.GetInstance()";

        public string GetCheckPredicateClause(string innerText)
        {
            if(AtLeastOneParentIsConditional)
            {
                return $"{ClassName}.CheckPredicate({innerText})";

            }

            return $"true";
        }

        public InstanceContainerGenerator(
            IDiagnosticReporter diagnosticReporter,
            BindingsContainer bindingsContainer,
            BindingContainer bindingContainer
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (bindingsContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingsContainer));
            }

            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            BindingContainer = bindingContainer;

            ClassName = $"{string.Join("_", bindingContainer.FromTypeNames)}_{bindingContainer.TargetTypeName}_{nameof(SingletonInstanceContainer)}_{Guid.NewGuid().ToString().ConvertMinusToGround()}";

            var cus = SyntaxFactory.ParseCompilationUnit(Resources.SingletonInstanceContainer);
            var uds = cus.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();

            var usings = uds.ConvertAll(r => r.GetText().ToString());
            usings.Add($"using {typeof(ResolutionContext).Namespace};");
            Usings = usings;

            AtLeastOneParentIsConditional = bindingContainer.IsConditional ||
                bindingsContainer.CheckForAtLeastOneParentIsConditional(
                    diagnosticReporter,
                    bindingContainer
                );
        }

        public string GetClassBody(
            InstanceContainerGeneratorsContainer container
            )
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var cus = SyntaxFactory.ParseCompilationUnit(Resources.SingletonInstanceContainer);
            var cds = cus.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

            var classBody = cds.GetText().ToString()
                .CheckAndReplace(nameof(SingletonInstanceContainer), ClassName)
                .CheckAndReplace(nameof(FakeTarget), BindingContainer.TargetTypeFullName)
                .CheckAndReplace("//GENERATOR: arguments", string.Join(",", BindingContainer.ConstructorArguments.Select(ca => ca.GetConstructorClause(container))))
                .CheckAndReplace("public sealed class", "private sealed class")
                .CheckAndReplaceIfTrue(() => AtLeastOneParentIsConditional, "#if UNDECLARED_SYMBOL", "#if !UNDECLARED_SYMBOL")
                ;

            return classBody;
        }
    }
}
