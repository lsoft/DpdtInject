using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Producer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed
{
    public class ConstantParsedBindExpression : BaseParsedBindExpression
    {
        private readonly SemanticModelDecorator _semanticModel;
        private readonly ExpressionStatementSyntax _expressionNode;

        private readonly List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> _invocationSymbols;
        private readonly ArgumentSyntax _constantClause;
        private readonly Tuple<InvocationExpressionSyntax, IMethodSymbol> _from;
        private readonly ImmutableArray<ITypeSymbol> _fromTypes;
        private readonly Tuple<InvocationExpressionSyntax, IMethodSymbol> _constScope;

        private readonly ArgumentSyntax? _whenArgumentClause;

        private bool _typesAlreadyBuild = false;


        public ConstantParsedBindExpression(
            SemanticModelDecorator semanticModel,
            ExpressionStatementSyntax expressionNode,
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols
            ) : base(BindScopeEnum.Constant, invocationSymbols)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (expressionNode is null)
            {
                throw new ArgumentNullException(nameof(expressionNode));
            }

            if (invocationSymbols is null)
            {
                throw new ArgumentNullException(nameof(invocationSymbols));
            }

            _semanticModel = semanticModel;
            _expressionNode = expressionNode;
            _invocationSymbols = invocationSymbols;

            var constantClause = DetermineArgumentSubClause(
                invocationSymbols,
                typeof(IToOrConstantBinding).GetMethod(nameof(IToOrConstantBinding.WithConstScope), BindingFlags.Public | BindingFlags.Instance)!
                );

            if (constantClause is null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Cannot find constant clause"
                    );
            }

            _constantClause = constantClause;

            _from = invocationSymbols.First(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(DefaultCluster).ToGlobalDisplayString() && s.Item2.Name == DefaultCluster.BindMethodName
                );
            _fromTypes = _from.Item2.TypeArguments;

            if (_fromTypes.Any(t => t is IDynamicTypeSymbol))
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom,
                    $"Dynamic cannot be used as bind from type"
                    );
            }

            _constScope = invocationSymbols.First(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IToOrConstantBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IToOrConstantBinding.WithConstScope)
                );

            //euristric if constant is a dynamic actually
            if (_constScope.Item2.TypeArguments[0].TypeKind == TypeKind.TypeParameter)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom,
                    $"Dynamic cannot be used as bind from type"
                    );
            }

            _whenArgumentClause = DetermineArgumentSubClause(
                invocationSymbols,
                typeof(IConditionalBinding).GetMethod(nameof(IConditionalBinding.When), BindingFlags.Public | BindingFlags.Instance)!,
                typeof(IConstantConditionalBinding).GetMethod(nameof(IConstantConditionalBinding.When), BindingFlags.Public | BindingFlags.Instance)!
                );
        }

        public override void Validate()
        {
            CheckForClusterTypes();
            CheckForAllowedSyntaxForConstantBinding();
        }


        public override IBindingContainer CreateBindingContainer(
            )
        {
            var types = BuildTypes();

            var bindingContainer = new ConstantBindingContainer(
                types,
                _constantClause,
                _scope,
                _expressionNode,
                _whenArgumentClause,
                _settings
                );

            return bindingContainer;
        }


        /// <summary>
        /// build appropriate types
        ///  also, if required: build factory source code, and rebuild factory symbol (we need an access to constructors)
        /// </summary>
        private BindingContainerTypes BuildTypes(
            )
        {
            if (_typesAlreadyBuild)
            {
                throw new InvalidOperationException($"types already build");
            }

            _typesAlreadyBuild = true;

            var constTypeSymbol = _constScope.Item2.TypeArguments[0];

            BindingContainerTypes types = new(
                _fromTypes,
                constTypeSymbol
                );

            return types;
        }


        private void CheckForClusterTypes()
        {
            foreach (var fromType in _fromTypes)
            {
                CheckForClusterType(fromType);
            }
        }

        private void CheckForAllowedSyntaxForConstantBinding(
            )
        {
            var child = _constantClause.ChildNodes().FirstOrDefault();
            if (child == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown error during validation a target in 'constant' binding."
                    );
            }

            var cconstant = _semanticModel.GetConstantValue(child);
            if (cconstant.HasValue)
            {
                //it's a true constant, keep going!
                return;
            }

            var ilsymbol = _semanticModel.GetSymbolInfo(child).Symbol;

            if (ilsymbol is null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown error during validation a target in 'constant' binding."
                    );
            }

            if (ilsymbol.Kind != SymbolKind.Field)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unsupported type of 'constant' binding. Allowed compile-time constants, readonly fields and readonly static fields only."
                    );
            }

            var filsymbol = ilsymbol as IFieldSymbol;

            if (filsymbol is null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown error during validation a target in 'constant' binding."
                    );
            }

            if (!filsymbol.IsReadOnly)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unsupported type of 'constant' binding. Allowed compile-time constants, readonly fields and readonly static fields only."
                    );
            }
        }
    }
}
