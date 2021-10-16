using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Injector.Src.Bind.Settings;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Generator.Core.Binding.Settings.Constructor;

namespace DpdtInject.Generator.Core.Binding
{
    public interface IBindingContainer
    {
        /// <summary>
        /// Unique identifier that may change between the compilations.
        /// MUST NOT used in cluster code generation!
        /// Used mostly in Dpdt Visual Studio Extension UI.
        /// </summary>
        Guid UniqueUnstableIdentifier
        {
            get;
        }

        /// <summary>
        /// Compile-time settings parsed from bind clause.
        /// </summary>
        IReadOnlyList<IDefinedSetting> Settings
        {
            get;
        }

        /// <summary>
        /// Bind from types. For example for
        /// <code>Bind<IA, IB>().To<ABClass>()...</code>
        /// this will contain <code>IA</code> and <code>IB</code>
        /// </summary>
        IReadOnlyList<ITypeSymbol> BindFromTypes
        {
            get;
        }

        /// <summary>
        /// Bind to type. For example for
        /// <code>Bind<IA, IB>().To<ABClass>()...</code>
        /// this will contain <code>ABClass</code>
        /// </summary>
        ITypeSymbol BindToType
        {
            get;
        }

        /// <summary>
        /// Visual and human readable target type representation.
        /// </summary>
        string TargetRepresentation
        {
            get;
        }

        /// <summary>
        /// Binding constructor arguments.
        /// Empty for a constant bindings.
        /// </summary>
        IReadOnlyList<DetectedMethodArgument> ConstructorArguments
        {
            get;
        }

        /// <summary>
        /// Bind scope <see cref="BindScopeEnum"/>
        /// </summary>
        BindScopeEnum Scope
        {
            get;
        }

        /// <summary>
        /// Is this binding conditional?
        /// (does this binding has a When predicate?)
        /// </summary>
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