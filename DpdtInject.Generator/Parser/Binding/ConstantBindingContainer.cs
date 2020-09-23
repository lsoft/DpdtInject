using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
using DpdtInject.Generator.Producer.Blocks.Cluster;
using DpdtInject.Generator.Properties;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Parser.Binding
{
    [DebuggerDisplay("{BindFromTypes[0].Name} -> {TargetRepresentation}")]
    public class ConstantBindingContainer : BaseBindingContainer
    {
        private readonly ArgumentSyntax _constantSyntax;

        public override IReadOnlyList<DetectedConstructorArgument> ConstructorArguments
        {
            get;
        }

        public override IReadOnlyCollection<ITypeSymbol> NotBindConstructorArgumentTypes
        {
            get;
        }

        public override string TargetRepresentation
        {
            get
            {
                if (IsRootCluster)
                {
                    return $"constant[{BindToType.GetFullName()}]";
                }

                return $"constant[{DeclaredClusterType!.GetFullName()} : {BindToType.GetFullName()}]";
            }
        }


        public ConstantBindingContainer(
            ITypeSymbol declaredClusterType,
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol constTypeSymbol,
            ArgumentSyntax constantSyntax,
            BindScopeEnum scope,
            ArgumentSyntax? whenArgumentClause
            ) : base(declaredClusterType, bindFromTypes, constTypeSymbol, scope, whenArgumentClause)
        {
            if (constantSyntax is null)
            {
                throw new ArgumentNullException(nameof(constantSyntax));
            }

            _constantSyntax = constantSyntax;
            ConstructorArguments = new List<DetectedConstructorArgument>();
            NotBindConstructorArgumentTypes = new HashSet<ITypeSymbol>();
        }

        public override string PrepareInstanceContainerCode(
            ClusterGeneratorTreeJoint clusterGeneratorJoint
            )
        {
            if (clusterGeneratorJoint is null)
            {
                throw new ArgumentNullException(nameof(clusterGeneratorJoint));
            }

            GetInstanceContainerBody(out var className, out var resource);

            var cus = SyntaxFactory.ParseCompilationUnit(resource);
            var cds = cus.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            var instanceContainerCode = cds.GetText().ToString();

            var result = instanceContainerCode
                .CheckAndReplace(className, GetContainerStableClassName())
                .CheckAndReplace(nameof(FakeTarget), BindToType.GetFullName())
                .CheckAndReplace(
                    "//GENERATOR: init constant",
                    $"{nameof(ConstantInstanceContainer.Instance)} = {_constantSyntax.GetText()};")
                .CheckAndReplace(
                    "//GENERATOR: predicate",
                    WhenArgumentClause?.ToString() ?? "rc => true"
                    )
                ;

            return result;
        }
    }
}
