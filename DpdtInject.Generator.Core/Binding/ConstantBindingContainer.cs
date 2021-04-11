using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector.Src.Bind.Settings;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Generator.Core.Binding
{
    [DebuggerDisplay("{BindFromTypes[0].Name} -> {TargetRepresentation}")]
    public class ConstantBindingContainer : BaseBindingContainer
    {
        public override IReadOnlyList<DetectedMethodArgument> ConstructorArguments
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
                return $"constant[{BindToType.ToGlobalDisplayString()}]";
            }
        }


        public ConstantBindingContainer(
            BindingContainerTypes types,
            ArgumentSyntax constantSyntax,
            BindScopeEnum scope,
            ExpressionStatementSyntax expressionNode,
            ArgumentSyntax? whenArgumentClause,
            IReadOnlyList<ISetting> settings
            ) : base(types, scope, expressionNode, whenArgumentClause, constantSyntax, settings, false)
        {
            ConstructorArguments = new List<DetectedMethodArgument>();
            NotBindConstructorArgumentTypes = new HashSet<ITypeSymbol>(
                TypeSymbolEqualityComparer.Entity
                );
        }
    }
}
