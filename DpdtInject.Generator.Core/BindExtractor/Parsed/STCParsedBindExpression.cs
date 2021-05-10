using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Generator.Core.Producer.ClassProducer;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src.Helper;
using DpdtInject.Injector.Src.Bind.Settings;
using DpdtInject.Generator.Core.Binding.Settings.Constructor;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed
{
    public class STCParsedBindExpression : BaseParsedBindExpression
    {
        private readonly ITypeInfoContainer _typeInfoContainer;
        private readonly ConstructorArgumentFromSyntaxExtractor _extractor;
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;
        private readonly ExpressionStatementSyntax _expressionNode;

        private readonly List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> _invocationSymbols;
        private readonly ImmutableArray<ITypeSymbol> _fromTypes;
        private readonly ITypeSymbol _toType;
        private readonly bool _isConventional;
        private readonly Tuple<InvocationExpressionSyntax, IMethodSymbol>? _factoryPayload;
        private readonly Tuple<InvocationExpressionSyntax, IMethodSymbol>? _proxySettings;
        private readonly ArgumentSyntax? _whenArgumentClause;

        private bool _typesAlreadyBuild = false;



        public STCParsedBindExpression(
            ITypeInfoContainer typeInfoContainer,
            ConstructorArgumentFromSyntaxExtractor extractor,
            ConstructorArgumentDetector constructorArgumentDetector,
            ExpressionStatementSyntax expressionNode,
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols,
            in ImmutableArray<ITypeSymbol> fromTypes,
            ITypeSymbol toType,
            BindScopeEnum scope,
            bool isConventional
            ) : base(scope, invocationSymbols)
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

            if (invocationSymbols is null)
            {
                throw new ArgumentNullException(nameof(invocationSymbols));
            }

            if (toType is null)
            {
                throw new ArgumentNullException(nameof(toType));
            }

            if (scope == BindScopeEnum.Constant)
            {
                throw new ArgumentOutOfRangeException(nameof(scope));
            }

            _typeInfoContainer = typeInfoContainer;
            _extractor = extractor;
            _constructorArgumentDetector = constructorArgumentDetector;
            _expressionNode = expressionNode;
            _invocationSymbols = invocationSymbols;

            _fromTypes = fromTypes;
            _toType = toType;
            _isConventional = isConventional;
            _factoryPayload = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IToFactoryBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IToFactoryBinding.WithPayload)
                );

            _proxySettings = invocationSymbols.FirstOrDefault(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IToProxyBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IToProxyBinding.WithProxySettings)
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
                CheckBindFromIsOnlyOneAndIsInterface();

                CheckForAbsenceForConfigureStatements();
            }
        }


        public override IBindingContainer CreateBindingContainer(
            )
        {
            var types = BuildTypes();

            var constructorArguments = GetConstructorArguments();

            _settings.TryGetSettingInScope<ConstructorSetting>(ConstructorSetting.ScopeConstant, out var constructorSetting);

            _constructorArgumentDetector.ChooseConstructorAndAppendUnknownArguments(
                (INamedTypeSymbol) types.BindToType,
                constructorSetting,
                ref constructorArguments
                );

            var bindingContainer = new BindingContainerWithInstance(
                types,
                constructorArguments,
                _scope,
                _expressionNode,
                _whenArgumentClause,
                _settings,
                _isConventional
                );

            return bindingContainer;
        }






        private List<DetectedMethodArgument> GetConstructorArguments()
        {
            _extractor.ClearAndVisit(_expressionNode);

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

            var updateTypes = false;
            if (_factoryPayload is not null)
            {
                if (!_typeInfoContainer.IsFactoryBuildFor(types.BindFromTypes[0]))
                {
                    var factoryProducer = new FactoryProducer(
                        types,
                        _factoryPayload.Item2.TypeArguments[0]
                        );

                    UpdateCompilation(types, factoryProducer);
                    
                    _typeInfoContainer.AddFactoryBuildFor(types.BindFromTypes[0]);
                }

                updateTypes = true;
            }
            else if (_proxySettings is not null)
            {
                if (!_typeInfoContainer.IsProxyBuildFor(types.BindFromTypes[0]))
                {
                    var proxyProducer = new ProxyProducer(
                        types,
                        _proxySettings.Item2.TypeArguments[0],
                        _proxySettings.Item2.TypeArguments[1]
                        );

                    UpdateCompilation(types, proxyProducer);
                    
                    _typeInfoContainer.AddProxyBuildFor(types.BindFromTypes[0]);
                }

                updateTypes = true;
            }

            if (updateTypes)
            {
                var updatedBindToType = _typeInfoContainer.GetTypeByMetadataName(
                    types.BindToType.ToFullDisplayString()
                    );

                if (updatedBindToType is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Cannot get an updated version of the class [{types.BindToType.ToGlobalDisplayString()}]",
                        types.BindToType.ToGlobalDisplayString()
                        );
                }

                types = new BindingContainerTypes(
                    _fromTypes,
                    updatedBindToType
                    );
            }

            return types;
        }

        private void UpdateCompilation(
            BindingContainerTypes types,
            IClassProducer classProducer
            )
        {
            var product = classProducer.GenerateProduct();

            var productSourceCode = product.GetSourceCode();

            _typeInfoContainer.AddSources(
                new []
                {
                    new ModificationDescription(
                        (INamedTypeSymbol) types.BindToType,
                        $"{types.BindToType.Name}.Pregenerated.cs",
                        productSourceCode
                        )
                }
                );
        }

        private void CheckBindFromIsOnlyOneAndIsInterface()
        {
            if (_fromTypes.Length != 1)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                    $"Proxy binding for [{_toType.ToGlobalDisplayString()}] should have 1 bind from clause.",
                    _toType.ToGlobalDisplayString()
                    );
            }

            if (_fromTypes[0].TypeKind != TypeKind.Interface)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                    $"Proxy binding for [{_toType.ToGlobalDisplayString()}] only support interfaces as bind-from, but [{_fromTypes[0].ToGlobalDisplayString()}] is not an interface.",
                    _toType.ToGlobalDisplayString()
                    );
            }

        }

        private void CheckForAbsenceForConfigureStatements()
        {
            var configures = _invocationSymbols.FindAll(
                s => s.Item2.ContainingType.ToGlobalDisplayString() == typeof(IConfigureBinding).ToGlobalDisplayString() && s.Item2.Name == nameof(IConfigureBinding.Configure)
                );

            if (configures.Count != 0)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                    $"Proxy binding for [{_toType.ToGlobalDisplayString()}] should have 0 {nameof(IConfigureBinding.Configure)} invocations. There is nothing to configure.",
                    _toType.ToGlobalDisplayString()
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
                    $"Type [{_toType.ToGlobalDisplayString()}] is not a class or struct, it is {_toType.Kind}",
                    _toType.ToGlobalDisplayString()
                    );
            }


            if (_factoryPayload is not null || _proxySettings is not null)
            {
                //it's a factory or a proxy

                if (_fromTypes.Length > 1)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom,
                        $"It is allowed only one bind-from-type for this kind of bindings. Take a look to [{_toType.ToGlobalDisplayString()}] binding.",
                        _toType.ToGlobalDisplayString()
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
                                $"Type [{_toType.ToGlobalDisplayString()}] must be partial",
                                _toType.ToGlobalDisplayString()
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
                    if (!_toType.CanBeCastedTo(bindFrom))
                    {
                        throw new DpdtException(
                            DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom,
                            $"Type [{_toType.ToGlobalDisplayString()}] cannot be casted to [{bindFrom.ToGlobalDisplayString()}]",
                            _toType.ToGlobalDisplayString()
                            );
                    }
                }

            }
        }
    }
}
