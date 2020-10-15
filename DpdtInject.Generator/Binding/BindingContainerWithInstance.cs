using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding
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
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol bindToType,
            IReadOnlyList<DetectedConstructorArgument> constructorArguments,
            BindScopeEnum scope,
            ArgumentSyntax? whenArgumentClause,
            ITypeSymbol? factoryPayloadType
            ) : base(bindFromTypes, bindToType, scope, whenArgumentClause, null, factoryPayloadType)
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
                new TypeSymbolEqualityComparer()
                );
        }
    }
}
