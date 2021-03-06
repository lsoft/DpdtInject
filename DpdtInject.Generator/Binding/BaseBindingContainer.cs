using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Binding
{
    public abstract class BaseBindingContainer : IBindingContainer
    {
        private readonly BindingContainerTypes _types;

        /// <inheritdoc />
        public Guid Identifier
        {
            get;
        }

        public IReadOnlyList<ITypeSymbol> BindFromTypes => _types.BindFromTypes;



        public ITypeSymbol BindToType => _types.BindToType;


        public abstract IReadOnlyList<DetectedConstructorArgument> ConstructorArguments
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

        public IReadOnlyCollection<string> FromTypeFullNames
        {
            get;
        }

        public bool IsConditional => WhenArgumentClause is not null;

        public abstract string TargetRepresentation
        {
            get;
        }


        protected BaseBindingContainer(
            BindingContainerTypes types,
            BindScopeEnum scope,
            ExpressionStatementSyntax expressionNode,
            ArgumentSyntax? whenArgumentClause,
            ArgumentSyntax? constantSyntax
            )
        {
            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (expressionNode is null)
            {
                throw new ArgumentNullException(nameof(expressionNode));
            }

            if (scope == BindScopeEnum.Constant && constantSyntax is null)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Misconfiguration between scope and constant syntax");
            }
            if(scope != BindScopeEnum.Constant && !(constantSyntax is null))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Misconfiguration between scope and constant syntax");
            }

            _types = types;
            Scope = scope;
            ExpressionNode = expressionNode;
            WhenArgumentClause = whenArgumentClause;
            ConstantSyntax = constantSyntax;

            Identifier = Guid.NewGuid();
            FromTypeFullNames = new HashSet<string>(BindFromTypes.ConvertAll(b => b.ToDisplayString()));
        }

        public string GetFromTypeFullNamesCombined(string separator = "_") => string.Join(separator, FromTypeFullNames);

        public string GetStableSuffix()
        {
            return this.GetHashCode().ToString();
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
