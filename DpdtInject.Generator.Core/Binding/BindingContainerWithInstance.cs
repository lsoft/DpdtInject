using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Bind.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core.Binding
{
    [DebuggerDisplay("{BindFromTypes[0].Name} -> {TargetRepresentation}")]
    public class BindingContainerWithInstance : BaseBindingContainer
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
                return BindToType.ToGlobalDisplayString();
            }
        }


        public BindingContainerWithInstance(
            BindingContainerTypes types,
            IReadOnlyList<DetectedMethodArgument> constructorArguments,
            BindScopeEnum scope,
            ExpressionStatementSyntax expressionNode,
            ArgumentSyntax? whenArgumentClause,
            IReadOnlyList<ISetting> settings,
            bool isConventional
            ) : base(types, scope, expressionNode, whenArgumentClause, null, settings, isConventional)
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
