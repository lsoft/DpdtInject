using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Bind.Settings;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core.Binding
{
    public abstract class BaseBindingContainer : IBindingContainer
    {
        private readonly string _uniqueKey = Guid.NewGuid().RemoveMinuses().Substring(0, 8);

        private readonly BindingContainerTypes _types;
        private readonly IReadOnlyList<ISetting> _settings;

        /// <inheritdoc />
        public Guid Identifier
        {
            get;
        }

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
            Identifier = Guid.NewGuid();
        }

        public bool IsSetup<T>()
             where T : class, ISetting
        {
            return _settings.Any(s => s.GetType().FullName == typeof(T).FullName!);
        }

        public bool TryGetSettingInScope<TScope>([NotNullWhen(true)] out TScope? setting)
             where TScope : class, ISetting
        {
            setting = _settings.FirstOrDefault(s => s.GetType().BaseType!.FullName == typeof(TScope).FullName!) as TScope;

            return setting != null;
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
