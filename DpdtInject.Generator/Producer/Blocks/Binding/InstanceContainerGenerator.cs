using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
using DpdtInject.Generator.Producer.Blocks.Cluster;
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

    public class InstanceContainerGenerator


    {
        public IBindingContainer BindingContainer
        {
            get;
        }

        public IReadOnlyList<ITypeSymbol> BindFromTypes => BindingContainer.BindFromTypes;

        public string ClassName => BindingContainer.GetContainerStableClassName();

        public IReadOnlyList<string> Usings
        {
            get;
        }

        public bool ItselfOrAtLeastOneChildIsConditional
        {
            get;
            set;
        }


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
            string clusterClassName,
            string innerText,
            DpdtArgumentWrapperTypeEnum wrapperType
            ) => $"{clusterClassName}.{ClassName}.GetInstance{wrapperType.GetPostfix()}({innerText})";


        public InstanceContainerGenerator(
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

            bindingContainer.GetInstanceContainerBody(out var _, out var resource);
            var cus = SyntaxFactory.ParseCompilationUnit(resource);
            var uds = cus.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();

            var usings = uds.ConvertAll(r => r.GetText().ToString());
            usings.Add($"using {typeof(ResolutionContext).Namespace};");
            Usings = usings;
        }


        public string GetClassBody(
            ClusterGeneratorTreeJoint clusterGeneratorJoint
            )
        {
            if (clusterGeneratorJoint is null)
            {
                throw new ArgumentNullException(nameof(clusterGeneratorJoint));
            }

            var classBody = BindingContainer.PrepareInstanceContainerCode(clusterGeneratorJoint)
                //.CheckAndReplace("public sealed class", "private sealed class")
                .CheckAndReplaceIfTrue(() => ItselfOrAtLeastOneChildIsConditional, "#if UNDECLARED_SYMBOL", "#if !UNDECLARED_SYMBOL")
                ;

            return classBody;
        }

        internal string GetVariableStableName()
        {
            return $"generator{this.GetHashCode()}";
        }
    }
}
