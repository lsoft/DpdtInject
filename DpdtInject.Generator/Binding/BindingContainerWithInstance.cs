using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Binding
{
    [DebuggerDisplay("{BindFromTypes[0].Name} -> {TargetRepresentation}")]
    public class BindingContainerWithInstance : BaseBindingContainer
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
                return BindToType.ToDisplayString();
            }
        }


        public BindingContainerWithInstance(
            BindingContainerTypes types,
            IReadOnlyList<DetectedConstructorArgument> constructorArguments,
            BindScopeEnum scope,
            ArgumentSyntax? whenArgumentClause
            ) : base(types, scope, whenArgumentClause, null)
        {
            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }

            ConstructorArguments = constructorArguments;
            NotBindConstructorArgumentTypes = new HashSet<ITypeSymbol>(
                constructorArguments
                    .Where(ca => !ca.DefineInBindNode)
                    .Select(ca => ca.Type!),
                TypeSymbolEqualityComparer.Entity
                );
        }
    }
}
