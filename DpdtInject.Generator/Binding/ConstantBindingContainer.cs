using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;

namespace DpdtInject.Generator.Binding
{
    [DebuggerDisplay("{BindFromTypes[0].Name} -> {TargetRepresentation}")]
    public class ConstantBindingContainer : BaseBindingContainer
    {
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
                return $"constant[{BindToType.ToDisplayString()}]";
            }
        }


        public ConstantBindingContainer(
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol constTypeSymbol,
            ArgumentSyntax constantSyntax,
            BindScopeEnum scope,
            ArgumentSyntax? whenArgumentClause
            ) : base(bindFromTypes, constTypeSymbol, scope, whenArgumentClause, constantSyntax, false)
        {
            ConstructorArguments = new List<DetectedConstructorArgument>();
            NotBindConstructorArgumentTypes = new HashSet<ITypeSymbol>();
        }
    }
}
