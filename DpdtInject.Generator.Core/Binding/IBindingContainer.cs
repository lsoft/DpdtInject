using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Injector.Src.Bind.Settings;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Generator.Core.Binding
{
    public interface IBindingContainer
    {
        Guid UniqueUnstableIdentifier
        {
            get;
        }

        IReadOnlyList<ISetting> Settings
        {
            get;
        }

        IReadOnlyList<ITypeSymbol> BindFromTypes
        {
            get;
        }


        ITypeSymbol BindToType
        {
            get;
        }

        string TargetRepresentation
        {
            get;
        }

        IReadOnlyList<DetectedMethodArgument> ConstructorArguments
        {
            get;
        }

        IReadOnlyCollection<ITypeSymbol> NotBindConstructorArgumentTypes
        {
            get;
        }

        BindScopeEnum Scope
        {
            get;
        }


        bool IsConditional
        {
            get;
        }

        /// <summary>
        /// This binding container is produced from conventional binding statement and
        /// has no explicit binding statement.
        /// </summary>
        bool IsConventional
        {
            get;
        }

        /// <summary>
        /// Binding expression syntax node. In case of conventional binding, it contains a conventional binding statement.
        /// </summary>
        ExpressionStatementSyntax ExpressionNode
        {
            get;
        }

        /// <summary>
        /// When argument syntax node. Null if there is no When predicate.
        /// Conventional bindings CAN have a When predicates.
        /// </summary>
        ArgumentSyntax? WhenArgumentClause
        {
            get;
        }

        /// <summary>
        /// Constant value syntax node. Null if it is NOT a constant binding 
        /// OR it is a conventional binding (which cannot be constant).
        /// </summary>
        ArgumentSyntax? ConstantSyntax
        {
            get;
        }


        string GetStableSuffix();

        bool IsRegisteredFrom(ITypeSymbol bindFrom);
    }
}