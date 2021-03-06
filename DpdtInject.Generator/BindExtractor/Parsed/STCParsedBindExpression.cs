using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.ClassProducer;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.BindExtractor.Parsed
{
    public class STCParsedBindExpression : BaseParsedBindExpression
    {
        private readonly ITypeInfoContainer _typeInfoContainer;
        private readonly ConstructorArgumentFromSyntaxExtractor _extractor;
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;

        private readonly List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> _invocationSymbols;
        private readonly Tuple<InvocationExpressionSyntax, IMethodSymbol> _from;
        private readonly ImmutableArray<ITypeSymbol> _fromTypes;
        private readonly Tuple<InvocationExpressionSyntax, IMethodSymbol> _to;
        private readonly ITypeSymbol _toType;
        private readonly Tuple<InvocationExpressionSyntax, IMethodSymbol>? _factoryPayload;
        private readonly Tuple<InvocationExpressionSyntax, IMethodSymbol>? _proxySettings;
        private readonly ArgumentSyntax? _whenArgumentClause;

        private bool _typesAlreadyBuild = false;

        public override ExpressionStatementSyntax ExpressionNode
        {
            get;
        }

        public override BindScopeEnum Scope
        {
            get;
        }


        public STCParsedBindExpression(
            ITypeInfoContainer typeInfoContainer,
            ConstructorArgumentFromSyntaxExtractor extractor,
            ConstructorArgumentDetector constructorArgumentDetector,
            ExpressionStatementSyntax expressionNode,
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols,
            BindScopeEnum scope
            )
        {
            if (typeInfoContainer is null)
            {
                throw new ArgumentNullException(nameof(typeInfoContainer));
            }

            if (extractor is null)
            {
                throw new ArgumentNullException(nameof(extractor));
            }

            if (constructorArgumentDetector is null)
            {
                throw new ArgumentNullException(nameof(constructorArgumentDetector));
            }

            if (expressionNode is null)
            {
                throw new ArgumentNullException(nameof(expressionNode));
            }

            if (invocationSymbols is null)
            {
                throw new ArgumentNullException(nameof(invocationSymbols));
            }

            if (scope == BindScopeEnum.Constant)
            {
                throw new ArgumentOutOfRangeException(nameof(scope));
            }

            _typeInfoContainer = typeInfoContainer;
            _extractor = extractor;
            _constructorArgumentDetector = constructorArgumentDetector;

            ExpressionNode = expressionNode;
            _invocationSymbols = invocationSymbols;
            Scope = scope;

            _from = invocationSymbols.First(
                s => s.Item2.ContainingType.ToDisplayString() == typeof(DefaultCluster).FullName && s.Item2.Name == DefaultCluster.BindMethodName
                );
            _fromTypes = _from.Item2.TypeArguments;


            _to = GetTo(invocationSymbols);
            _toType = _to.Item2.TypeArguments.First();

            _factoryPayload = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToDisplayString() == typeof(IToFactoryBinding).FullName && s.Item2.Name == nameof(IToFactoryBinding.WithPayload)
                );

            _proxySettings = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToDisplayString() == typeof(IToProxyBinding).FullName && s.Item2.Name == nameof(IToProxyBinding.WithProxySettings)
                );

            _whenArgumentClause = DetermineArgumentSubClause(
                invocationSymbols,
                typeof(IConditionalBinding).GetMethod(nameof(IConditionalBinding.When), BindingFlags.Public | BindingFlags.Instance)!,
                typeof(IConstantConditionalBinding).GetMethod(nameof(IConstantConditionalBinding.When), BindingFlags.Public | BindingFlags.Instance)!
                );
        }

        public override void Validate()
        {
            CheckForFromAndToTypes();

            if (_proxySettings is not null)
            {
                CheckForAbsenceForConfigureStatements();
            }
        }


        public override IBindingContainer CreateBindingContainer(
            )
        {
            var types = BuildTypes();

            var constructorArguments = GetConstructorArguments();

            _constructorArgumentDetector.AppendUnknown(
                (INamedTypeSymbol) types.BindToType,
                ref constructorArguments
                );

            var bindingContainer = new BindingContainerWithInstance(
                types,
                constructorArguments,
                Scope,
                ExpressionNode,
                _whenArgumentClause
                );

            return bindingContainer;
        }






        private List<DetectedConstructorArgument> GetConstructorArguments()
        {
            _extractor.ClearAndVisit(ExpressionNode);

            var constructorArguments = _extractor.GetConstructorArguments();

            return constructorArguments;
        }

        /// <summary>
        /// build appropriate types
        ///  also, if required: build factory/proxy source code, and rebuild factory/proxy symbol (we need an access to constructors)
        /// </summary>
        private BindingContainerTypes BuildTypes(
            )
        {
            if (_typesAlreadyBuild)
            {
                throw new InvalidOperationException($"types already build");
            }

            _typesAlreadyBuild = true;

            var types = new BindingContainerTypes(
                _fromTypes,
                _toType
                );

            IClassProducer? classProducer =  null;
            if (_factoryPayload is not null)
            {
                classProducer = new FactoryProducer(
                    types,
                    _factoryPayload.Item2.TypeArguments[0]
                    );
            }
            else if (_proxySettings is not null)
            {
                classProducer = new ProxyProducer(
                    types,
                    _proxySettings.Item2.TypeArguments[0],
                    _proxySettings.Item2.TypeArguments[1]
                    );
            }

            if (classProducer is not null)
            {
                var product = classProducer.GenerateProduct();

                _typeInfoContainer.AddSources(
                    new ModificationDescription[]
                    {
                        new(
                            (INamedTypeSymbol) types.BindToType,
                            $"{types.BindToType.Name}.Pregenerated.cs",
                            product.SourceCode
                            )
                    }
                    );

                var updatedBindToType = _typeInfoContainer.GetTypeByMetadataName(
                    types.BindToType.ToDisplayString()
                    );

                if (updatedBindToType is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Cannot get an updated version of the class [{types.BindToType.ToDisplayString()}]",
                        types.BindToType.ToDisplayString()
                        );
                }

                types = new BindingContainerTypes(
                    _fromTypes,
                    updatedBindToType
                    );
            }

            return types;
        }



        private void CheckForAbsenceForConfigureStatements()
        {
            var configures = _invocationSymbols.FindAll(
                s => s.Item2.ContainingType.ToDisplayString() == typeof(IConfigureBinding).FullName && s.Item2.Name == nameof(IConfigureBinding.Configure)
                );

            if (configures.Count != 0)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                    $"Proxy binding for [{_toType.ToDisplayString()}] should have 0 {nameof(IConfigureBinding.Configure)} invocations. There is nothing to configure.",
                    _toType.ToDisplayString()
                    );
            }
        }


        private void CheckForFromAndToTypes(
            )
        {
            //check for target type correct
            if (_toType.TypeKind.NotIn(TypeKind.Class, TypeKind.Struct))
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectTarget,
                    $"Type [{_toType.ToDisplayString()}] is not a class or struct",
                    _toType.ToDisplayString()
                    );
            }


            if (_factoryPayload is not null || _proxySettings is not null)
            {
                //it's a factory or a proxy

                if (_fromTypes.Length > 1)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom,
                        $"It is allowed only one bind-from-type for this kind of bindings. Take a look to [{_toType.ToDisplayString()}] binding.",
                        _toType.ToDisplayString()
                        );
                }

                //check for partial clause
                if (_toType.DeclaringSyntaxReferences.Length > 1)
                {
                    //it's partial!
                }
                else
                {
                    var syntax = _toType.DeclaringSyntaxReferences[0].GetSyntax();
                    if (syntax is ClassDeclarationSyntax cds)
                    {
                        if (!cds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
                        {
                            throw new DpdtException(
                                DpdtExceptionTypeEnum.TargetClassMustBePartial,
                                $"Type [{_toType.ToDisplayString()}] must be partial",
                                _toType.ToDisplayString()
                                );
                        }
                    }
                }
            }

            if (_factoryPayload is null && _proxySettings is null)
            {
                //it's nor a factory neither a proxy
                //it's regular binding

                //check for cast exists
                foreach (var bindFrom in _fromTypes)
                {
                    if (!_toType.CanBeCastedTo(bindFrom.ToDisplayString()))
                    {
                        throw new DpdtException(
                            DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom,
                            $"Type [{_toType.ToDisplayString()}] cannot be casted to [{bindFrom.ToDisplayString()}]",
                            _toType.ToDisplayString()
                            );
                    }
                }

            }
        }


        private static Tuple<InvocationExpressionSyntax, IMethodSymbol> GetTo(
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols
            )
        {
            var pair = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToDisplayString() == typeof(IToOrConstantBinding).FullName && s.Item2.Name == nameof(IToOrConstantBinding.To)
                );

            if (pair is not null)
            {
                return pair;
            }

            pair = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToDisplayString() == typeof(IToOrConstantBinding).FullName && s.Item2.Name == nameof(IToOrConstantBinding.ToIsolatedFactory)
                );

            if (pair is not null)
            {
                return pair;
            }

            pair = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToDisplayString() == typeof(IToOrConstantBinding).FullName && s.Item2.Name == nameof(IToOrConstantBinding.ToProxy)
                );

            if (pair is not null)
            {
                return pair;
            }

            throw new DpdtException(
                DpdtExceptionTypeEnum.InternalError,
                $"Unknown problem to access 'To' clause"
                );
        }

    }
}
