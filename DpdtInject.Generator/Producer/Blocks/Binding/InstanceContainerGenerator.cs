using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
using DpdtInject.Generator.Properties;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Binding
{
    public class InstanceContainerGenerator
    {
        private readonly IReadOnlyCollection<string> _fromTypeNames;
        private readonly string _targetTypeName;
        private readonly string _targetTypeFullName;
        private readonly IReadOnlyList<DetectedConstructorArgument> _constructorArguments;

        public string ClassName
        {
            get;
        }

        public IReadOnlyList<string> Usings
        {
            get;
        }


        public string DisposeClause => $"{ClassName}.{nameof(SingletonInstanceContainer.DoDisposeIfApplicable)}();";

        public InstanceContainerGenerator(
            IReadOnlyCollection<string> fromTypeNames,
            string targetTypeName,
            string targetTypeFullName,
            IReadOnlyList<DetectedConstructorArgument> constructorArguments
            )
        {
            if (fromTypeNames is null)
            {
                throw new ArgumentNullException(nameof(fromTypeNames));
            }

            if (targetTypeName is null)
            {
                throw new ArgumentNullException(nameof(targetTypeName));
            }

            if (targetTypeFullName is null)
            {
                throw new ArgumentNullException(nameof(targetTypeFullName));
            }

            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }

            _fromTypeNames = fromTypeNames;
            _targetTypeName = targetTypeName;
            _targetTypeFullName = targetTypeFullName;
            _constructorArguments = constructorArguments;

            ClassName = $"{string.Join("_", fromTypeNames)}_{targetTypeName}_{nameof(SingletonInstanceContainer)}_{Guid.NewGuid().ToString().ConvertMinusToGround()}";

            var cus = SyntaxFactory.ParseCompilationUnit(Resources.SingletonInstanceContainer);
            var uds = cus.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();

            var usings = uds.ConvertAll(r => r.GetText().ToString());
            usings.Add($"using {typeof(ResolutionContext).Namespace};");
            Usings = usings;
        }

        public string GetClassBody(
            BindingsContainer bindingProcessorContainer
            )
        {
            if (bindingProcessorContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingProcessorContainer));
            }

            var cus = SyntaxFactory.ParseCompilationUnit(Resources.SingletonInstanceContainer);
            var cds = cus.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

            var classBody = cds.GetText().ToString()
                .CheckAndReplace(nameof(SingletonInstanceContainer), ClassName)
                .CheckAndReplace(nameof(FakeTarget), _targetTypeFullName)
                .CheckAndReplace("//GENERATOR: arguments", string.Join(",", _constructorArguments.Select(ca => ca.GetConstructorClause(bindingProcessorContainer))))
                .CheckAndReplace("public sealed class", "private sealed class")
                ;

            return classBody;
        }
    }
}
