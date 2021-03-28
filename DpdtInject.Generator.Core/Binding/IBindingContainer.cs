﻿using System;
using System.Collections.Generic;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Bind.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core.Binding
{
    public interface IBindingContainer : ISettingsProvider
    {
        Guid Identifier
        {
            get;
        }

        IReadOnlyList<ITypeSymbol> BindFromTypes
        {
            get;
        }

        IReadOnlyCollection<string> FromTypeFullNames
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

        IReadOnlyList<DetectedConstructorArgument> ConstructorArguments
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
        /// Binding expression syntax node. Null if it is a conventional binding.
        /// </summary>
        ExpressionStatementSyntax? ExpressionNode
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


        string GetFromTypeFullNamesCombined(string separator = "_");

        string GetStableSuffix();

        bool IsRegisteredFrom(ITypeSymbol bindFrom);
    }
}