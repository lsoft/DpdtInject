using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DpdtInject.Generator.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Injector.Src.Bind.Settings;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Excp;
using System.Security.Cryptography;

namespace DpdtInject.Generator.Core.Binding
{
    public abstract class BaseBindingContainer : IBindingContainer
    {
        private readonly string _uniqueKey;

        private readonly BindingContainerTypes _types;
        private readonly IReadOnlyList<ISetting> _settings;

        /// <inheritdoc />
        public Guid UniqueUnstableIdentifier
        {
            get;
        }

        public IReadOnlyList<ISetting> Settings => _settings;

        public IReadOnlyList<ITypeSymbol> BindFromTypes => _types.BindFromTypes;



        public ITypeSymbol BindToType => _types.BindToType;


        public abstract IReadOnlyList<DetectedMethodArgument> ConstructorArguments
        {
            get;
        }

        public abstract IReadOnlyCollection<ITypeSymbol> NotBindConstructorArgumentTypes
        {
            get;
        }

        public BindScopeEnum Scope
        {
            get;
        }

        public ExpressionStatementSyntax ExpressionNode
        {
            get;
        }


        public ArgumentSyntax? WhenArgumentClause
        {
            get;
        }

        public ArgumentSyntax? ConstantSyntax
        {
            get;
        }

        public bool IsConditional => WhenArgumentClause is not null;

        public abstract string TargetRepresentation
        {
            get;
        }

        /// <inheritdoc/>
        public bool IsConventional
        {
            get;
        }

        protected BaseBindingContainer(
            BindingContainerTypes types,
            BindScopeEnum scope,
            ExpressionStatementSyntax expressionNode,
            ArgumentSyntax? whenArgumentClause,
            ArgumentSyntax? constantSyntax,
            IReadOnlyList<ISetting> settings,
            bool isConventional
            )
        {
            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (scope == BindScopeEnum.Constant && constantSyntax is null)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Misconfiguration between scope and constant syntax");
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (scope != BindScopeEnum.Constant && !(constantSyntax is null))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Misconfiguration between scope and constant syntax");
            }

            _types = types;
            Scope = scope;
            ExpressionNode = expressionNode;
            WhenArgumentClause = whenArgumentClause;
            ConstantSyntax = constantSyntax;
            _settings = settings;
            IsConventional = isConventional;
            UniqueUnstableIdentifier = Guid.NewGuid();

            var uniqueKey0 = expressionNode.SyntaxTree.FilePath.GetStringSha256Hash().SafeSubstring(0, 8);
            var uniqueKey1 = expressionNode.Span.Start;
            var uniqueKey2 = expressionNode.Span.End;
            _uniqueKey = $"u{uniqueKey0}_{uniqueKey1}_{uniqueKey2}";
        }


        public string GetStableSuffix()
        {
            return _uniqueKey;
        }

        public bool IsRegisteredFrom(ITypeSymbol bindFrom)
        {
            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            return BindFromTypes.Any(t => SymbolEqualityComparer.Default.Equals(t, bindFrom));
        }
    }
}
