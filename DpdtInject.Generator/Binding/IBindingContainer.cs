using System;
using System.Collections.Generic;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Bind.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Binding
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

        ExpressionStatementSyntax ExpressionNode
        {
            get;
        }

        ArgumentSyntax? WhenArgumentClause
        {
            get;
        }

        ArgumentSyntax? ConstantSyntax
        {
            get;
        }


        string GetFromTypeFullNamesCombined(string separator = "_");

        string GetStableSuffix();

        bool IsRegisteredFrom(ITypeSymbol bindFrom);
    }
}