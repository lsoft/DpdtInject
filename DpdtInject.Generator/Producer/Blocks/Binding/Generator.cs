using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Generator.Properties;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DpdtInject.Generator.Producer.Blocks.Binding
{

    public class Generator

    {
        public IBindingContainer BindingContainer
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

        public bool ItselfOrAtLeastOneChildIsConditional =>
                BindingContainer.IsConditional
                || BindingContainer.AtLeastOneChildIsConditional
            ;


        public string DisposeClause
        {
            get
            {
                if (BindingContainer.Scope.In(BindScopeEnum.Singleton))
                {
                    return $"{ClassName}.{nameof(SingletonInstanceContainer.DoDisposeIfApplicable)}()";
                }

                return string.Empty;
            }
        }

        public string GetInstanceClause(
            string innerText,
            DpdtArgumentWrapperTypeEnum wrapperType
            ) => $"{ClassName}.GetInstance{wrapperType.GetPostfix()}({innerText})";


        public Generator(
            IDiagnosticReporter diagnosticReporter,
            IBindingContainer bindingContainer
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            BindingContainer = bindingContainer;

            switch (this.BindingContainer.Scope)
            {
                case Injector.Module.Bind.BindScopeEnum.Transient:
                    ClassName = $"{string.Join("_", bindingContainer.GetFromTypeFullNamesCombined().ConvertDotLessGreatherToGround())}_{bindingContainer.BindToType.GetFullName().ConvertDotLessGreatherToGround()}_{nameof(TransientInstanceContainer)}_{Guid.NewGuid().RemoveMinuses()}";
                    break;
                case Injector.Module.Bind.BindScopeEnum.Singleton:
                    ClassName = $"{string.Join("_", bindingContainer.GetFromTypeFullNamesCombined().ConvertDotLessGreatherToGround())}_{bindingContainer.BindToType.GetFullName().ConvertDotLessGreatherToGround()}_{nameof(SingletonInstanceContainer)}_{Guid.NewGuid().RemoveMinuses()}";
                    break;
                case Injector.Module.Bind.BindScopeEnum.Constant:
                    ClassName = $"{string.Join("_", bindingContainer.GetFromTypeFullNamesCombined().ConvertDotLessGreatherToGround())}_{nameof(ConstantInstanceContainer)}_{Guid.NewGuid().RemoveMinuses()}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            var cus = SyntaxFactory.ParseCompilationUnit(Resources.SingletonInstanceContainer);
            var uds = cus.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();

            var usings = uds.ConvertAll(r => r.GetText().ToString());
            usings.Add($"using {typeof(ResolutionContext).Namespace};");
            Usings = usings;
        }


        public string GetClassBody(
            GeneratorsContainer container
            )
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            string className;
            string resource;
            switch (this.BindingContainer.Scope)
            {
                case Injector.Module.Bind.BindScopeEnum.Transient:
                    className = nameof(TransientInstanceContainer);
                    resource = Resources.TransientInstanceContainer;
                    break;
                case Injector.Module.Bind.BindScopeEnum.Singleton:
                    className = nameof(SingletonInstanceContainer);
                    resource = Resources.SingletonInstanceContainer;
                    break;
                case Injector.Module.Bind.BindScopeEnum.Constant:
                    className = nameof(ConstantInstanceContainer);
                    resource = Resources.ConstantInstanceContainer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var cus = SyntaxFactory.ParseCompilationUnit(resource);
            var cds = cus.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

            var classBody = cds.GetText().ToString()
                .CheckAndReplace(className, ClassName)
                .CheckAndReplace("public sealed class", "private sealed class")
                .CheckAndReplaceIfTrue(() => ItselfOrAtLeastOneChildIsConditional, "#if UNDECLARED_SYMBOL", "#if !UNDECLARED_SYMBOL")
                .PrepareInstanceContainerCode(BindingContainer, container)
                ;

            return classBody;
        }

        internal string GetVariableStableName()
        {
            return $"generator{this.GetHashCode()}";
        }
    }

    public static class InstanceContainerGeneratorHelper
    {
        public static string PrepareInstanceContainerCode(
            this string instanceContainerCode,
            IBindingContainer bindingContainer,
            GeneratorsContainer container
            )
        {
            if (instanceContainerCode is null)
            {
                throw new ArgumentNullException(nameof(instanceContainerCode));
            }

            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return bindingContainer.PrepareInstanceContainerCode(instanceContainerCode, container);
        }
    }
}
