using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Factory;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;

namespace DpdtInject.Generator.BindExtractor
{
    public class DefaultBindExtractor : CSharpSyntaxRewriter
    {
        public static readonly string ComplexSeparator = "," + Environment.NewLine;

        private readonly ITypeInfoContainer _typeInfoContainer;
        private readonly ConstructorArgumentFromSyntaxExtractor _extractor;
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;
        private readonly SemanticModel _semanticModel;

        private readonly List<IBindingContainer> _bindingContainers;

        public DefaultBindExtractor(
            ITypeInfoContainer typeInfoContainer,
            SemanticModel semanticModel,
            ConstructorArgumentFromSyntaxExtractor extractor,
            ConstructorArgumentDetector constructorArgumentDetector
            )
        {
            if (typeInfoContainer is null)
            {
                throw new ArgumentNullException(nameof(typeInfoContainer));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (extractor is null)
            {
                throw new ArgumentNullException(nameof(extractor));
            }

            if (constructorArgumentDetector is null)
            {
                throw new ArgumentNullException(nameof(constructorArgumentDetector));
            }

            _typeInfoContainer = typeInfoContainer;
            _semanticModel = semanticModel;
            _extractor = extractor;
            _constructorArgumentDetector = constructorArgumentDetector;

            _bindingContainers = new List<IBindingContainer>();
        }


        public ClusterBindings GetClusterBindings(ITypeSymbol clusterType)
        {
            return
                new ClusterBindings(
                    clusterType,
                    _bindingContainers
                    );
        }

        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax expressionNode)
        {
            if (expressionNode is null)
            {
                throw new ArgumentNullException(nameof(expressionNode));
            }

            var genericNodes = expressionNode
                .DescendantNodes()
                .OfType<GenericNameSyntax>()
                .ToList();

            if (genericNodes.Count == 0)
            {
                return base.VisitExpressionStatement(expressionNode)!;
            }

            var bindGenericNode = genericNodes[0];
            var bindMethodName = bindGenericNode.Identifier.Text;
            if (bindMethodName != "Bind")
            {
                return base.VisitExpressionStatement(expressionNode)!;
            }

            var expressionText = expressionNode.GetText().ToString();

            if (!expressionText.Contains(nameof(IToOrConstantBinding.WithConstScope)))
            {
                if (!expressionText.Contains(nameof(IScopeBinding.WithSingletonScope)))
                {
                    if (!expressionText.Contains(nameof(IScopeBinding.WithTransientScope)))
                    {
                        if (!expressionText.Contains(nameof(IScopeBinding.WithCustomScope)))
                        {
                            return base.VisitExpressionStatement(expressionNode)!;
                        }
                    }
                }
            }

            //looks like we found what we want

            var scope = DetermineScope(expressionNode);

            var whenArgumentClause = DetermineArgumentSubClause(
                expressionNode,
                nameof(IConditionalBinding.When)
                );

            switch (scope)
            {
                case BindScopeEnum.Singleton:
                    ProcessSingleton(
                        expressionNode,
                        bindGenericNode,
                        whenArgumentClause
                        );
                    break;
                case BindScopeEnum.Transient:
                    ProcessTransient(
                        expressionNode,
                        bindGenericNode,
                        whenArgumentClause
                        );
                    break;
                case BindScopeEnum.Custom:
                    ProcessCustom(
                        expressionNode,
                        bindGenericNode,
                        whenArgumentClause
                        );
                    break;
                case BindScopeEnum.Constant:
                    ProcessConstant(
                        expressionNode,
                        bindGenericNode,
                        whenArgumentClause
                        );
                    break;
                default:
                    throw new DpdtException(DpdtExceptionTypeEnum.UnknownScope, $"Unknown scope {scope}");
            }

            return expressionNode;
        }

        private void ProcessSingleton(
            ExpressionStatementSyntax expressionNode,
            GenericNameSyntax bindGenericNode,
            ArgumentSyntax? whenArgumentClause
            )
        {
            var genericNodes = expressionNode
                .DescendantNodes()
                .OfType<GenericNameSyntax>()
                .ToList();

            var toGenericNodes = genericNodes
                .Where(ToMethodPredicate)
                .ToList()
                ;

            if (toGenericNodes.Count != 1)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Cannot find To clause for singleton binding : {toGenericNodes.Count}");
            }

            var toGenericNode = toGenericNodes[0];

            var toMethodName = toGenericNode.Identifier.Text;
            if (toMethodName.NotIn(nameof(IToOrConstantBinding.To), nameof(IToOrConstantBinding.ToIsolatedFactory)))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, "Cannot find To clause for singleton binding");
            }

            var factoryPayloadGenericNode = genericNodes.FirstOrDefault(gn => gn.Identifier.Text == nameof(IToFactoryBinding.WithPayload));

            var factoryPayloadSemantic = GetFactoryPayloadIfExists(
                factoryPayloadGenericNode
                );

            var bindFromTypeSemantics = GetBindFromTypes(
                bindGenericNode
                );

            var bindToSyntax = toGenericNode.TypeArgumentList.Arguments.First();
            ITypeSymbol? bindToTypeSemantic = _semanticModel.GetTypeInfo(bindToSyntax).Type;
            if (bindToTypeSemantic == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access {nameof(bindToTypeSemantic)}"
                    );
            }

            CheckForFromAndToTypes(
                bindFromTypeSemantics,
                bindToTypeSemantic,
                !(factoryPayloadSemantic is null)
                );

            //var fullBindToTypeName = _typeInfoContainer.GetTypeByMetadataName(bindToTypeSemantic.ToDisplayString());
            //if (fullBindToTypeName == null)
            //{
            //    throw new DpdtException(
            //        DpdtExceptionTypeEnum.InternalError,
            //        $"Unknown problem to access type for {bindToTypeSemantic.ToDisplayString()}"
            //        );
            //}

            _extractor.ClearAndVisit(expressionNode);

            var types = BuildTypes(
                bindFromTypeSemantics, 
                bindToTypeSemantic,
                factoryPayloadSemantic
                );

            var constructorArguments = _extractor.GetConstructorArguments();

            _constructorArgumentDetector.AppendUnknown(
                (INamedTypeSymbol)types.BindToType,
                ref constructorArguments
                );

            var bindingContainer = new BindingContainerWithInstance(
                types,
                constructorArguments,
                BindScopeEnum.Singleton,
                whenArgumentClause
                );

            _bindingContainers.Add(bindingContainer);
        }

        private void ProcessTransient(
            ExpressionStatementSyntax expressionNode,
            GenericNameSyntax bindGenericNode,
            ArgumentSyntax? whenArgumentClause
            )
        {
            var genericNodes = expressionNode
                .DescendantNodes()
                .OfType<GenericNameSyntax>()
                .ToList();

            var toGenericNodes = genericNodes
                .Where(ToMethodPredicate)
                .ToList()
                ;

            if (toGenericNodes.Count != 1)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Cannot find To clause for singleton binding : {toGenericNodes.Count}");
            }

            var toGenericNode = toGenericNodes[0];

            var toMethodName = toGenericNode.Identifier.Text;
            if (toMethodName.NotIn(nameof(IToOrConstantBinding.To), nameof(IToOrConstantBinding.ToIsolatedFactory)))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, "Cannot find To clause for transient binding");
            }

            var factoryPayloadGenericNode = genericNodes.FirstOrDefault(gn => gn.Identifier.Text == nameof(IToFactoryBinding.WithPayload));

            var factoryPayloadSemantic = GetFactoryPayloadIfExists(
                factoryPayloadGenericNode
                );

            var bindFromTypeSemantics = GetBindFromTypes(
                bindGenericNode
                );

            var bindToSyntax = toGenericNode.TypeArgumentList.Arguments.First();
            var bindToTypeSemantic = _semanticModel.GetTypeInfo(bindToSyntax).Type;
            if (bindToTypeSemantic == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access {nameof(bindToTypeSemantic)}"
                    );
            }

            CheckForFromAndToTypes(
                bindFromTypeSemantics,
                bindToTypeSemantic,
                !(factoryPayloadSemantic is null)
                );

            //var fullBindToTypeName = _typeInfoContainer.GetTypeByMetadataName(bindToTypeSemantic.ToDisplayString());
            //if (fullBindToTypeName == null)
            //{
            //    throw new DpdtException(
            //        DpdtExceptionTypeEnum.InternalError,
            //        $"Unknown problem to access type for {bindToTypeSemantic.ToDisplayString()}"
            //        );
            //}

            _extractor.ClearAndVisit(expressionNode);

            var types = BuildTypes(
                bindFromTypeSemantics,
                bindToTypeSemantic,
                factoryPayloadSemantic
                );

            var constructorArguments = _extractor.GetConstructorArguments();

            _constructorArgumentDetector.AppendUnknown(
                (INamedTypeSymbol)types.BindToType,
                ref constructorArguments
                );

            var bindingContainer = new BindingContainerWithInstance(
                types,
                constructorArguments,
                BindScopeEnum.Transient,
                whenArgumentClause
                );

            _bindingContainers.Add(bindingContainer);
        }

        private void ProcessCustom(
            ExpressionStatementSyntax expressionNode,
            GenericNameSyntax bindGenericNode,
            ArgumentSyntax? whenArgumentClause
            )
        {
            var genericNodes = expressionNode
                .DescendantNodes()
                .OfType<GenericNameSyntax>()
                .ToList();

            var toGenericNodes = genericNodes
                .Where(ToMethodPredicate)
                .ToList()
                ;

            if (toGenericNodes.Count != 1)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Cannot find To clause for singleton binding : {toGenericNodes.Count}");
            }

            var toGenericNode = toGenericNodes[0];

            var toMethodName = toGenericNode.Identifier.Text;
            if (toMethodName.NotIn(nameof(IToOrConstantBinding.To), nameof(IToOrConstantBinding.ToIsolatedFactory)))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, "Cannot find To clause for custom binding");
            }

            var factoryPayloadGenericNode = genericNodes.FirstOrDefault(gn => gn.Identifier.Text == nameof(IToFactoryBinding.WithPayload));

            var factoryPayloadSemantic = GetFactoryPayloadIfExists(
                factoryPayloadGenericNode
                );

            var bindFromTypeSemantics = GetBindFromTypes(
                bindGenericNode
                );

            var bindToSyntax = toGenericNode.TypeArgumentList.Arguments.First();
            var bindToTypeSemantic = _semanticModel.GetTypeInfo(bindToSyntax).Type;

            if (bindToTypeSemantic == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access {nameof(bindToTypeSemantic)}"
                    );
            }

            CheckForFromAndToTypes(
                bindFromTypeSemantics,
                bindToTypeSemantic,
                !(factoryPayloadSemantic is null)
                );

            //var fullBindToTypeName = _typeInfoContainer.GetTypeByMetadataName(bindToTypeSemantic.ToDisplayString());
            //if (fullBindToTypeName == null)
            //{
            //    throw new DpdtException(
            //        DpdtExceptionTypeEnum.InternalError,
            //        $"Unknown problem to access type for {bindToTypeSemantic.ToDisplayString()}"
            //        );
            //}

            _extractor.ClearAndVisit(expressionNode);

            var types = BuildTypes(
                bindFromTypeSemantics,
                bindToTypeSemantic,
                factoryPayloadSemantic
                );

            var constructorArguments = _extractor.GetConstructorArguments();

            _constructorArgumentDetector.AppendUnknown(
                (INamedTypeSymbol)types.BindToType,
                ref constructorArguments
                );

            var bindingContainer = new BindingContainerWithInstance(
                types,
                constructorArguments,
                BindScopeEnum.Custom,
                whenArgumentClause
                );

            _bindingContainers.Add(bindingContainer);
        }

        private void ProcessConstant(
            ExpressionStatementSyntax expressionNode,
            GenericNameSyntax bindGenericNode,
            ArgumentSyntax? whenArgumentClause
            )
        {
            var withScopeSyntax = expressionNode
                .DescendantNodes()
                .Where(s => s.GetText().ToString() == nameof(IToOrConstantBinding.WithConstScope))
                .First();

            var constTypeSymbol = (_semanticModel.GetSymbolInfo(withScopeSyntax).Symbol as IMethodSymbol)!.TypeArguments[0];

            var constantClause = DetermineArgumentSubClause(
                expressionNode,
                nameof(IToOrConstantBinding.WithConstScope)
                );

            if (constantClause is null)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Cannot find constant clause");
            }

            var bindFromTypeSemantics = GetBindFromTypes(
                bindGenericNode
                );

            var types = new BindingContainerTypes(
                bindFromTypeSemantics,
                constTypeSymbol,
                null
                );

            var bindingContainer = new ConstantBindingContainer(
                types,
                constantClause,
                BindScopeEnum.Constant,
                whenArgumentClause
                );

            _bindingContainers.Add(bindingContainer);
        }



        /// <summary>
        /// build appropriate types
        ///  also, if required: build factory source code, and rebuild factory symbol (we need to access to constructors)
        /// </summary>
        private BindingContainerTypes BuildTypes(
            List<ITypeSymbol> bindFromTypeSemantics,
            ITypeSymbol bindToTypeSemantic,
            ITypeSymbol? factoryPayloadSemantic
            )
        {
            var types = new BindingContainerTypes(
                bindFromTypeSemantics,
                bindToTypeSemantic,
                factoryPayloadSemantic
                );

            if (types.ToFactory)
            {
                var factoryProducer = new FactoryProducer(
                    types
                    );

                var factoryProduct = factoryProducer.GenerateFactoryProduct();

                _typeInfoContainer.AddSources(
                    new ModificationDescription[] 
                    {
                        new ModificationDescription(
                            (INamedTypeSymbol)types.BindToType,
                            $"{types.BindToType.Name}.Pregenerated.cs",
                            factoryProduct.SourceCode
                            )
                    }
                    );

                var updatedBindToTypeSemantic = _typeInfoContainer.GetTypeByMetadataName(
                    types.BindToType.ToDisplayString()
                    );

                if (updatedBindToTypeSemantic is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Cannot get an updated version of the factory class [{types.BindToType.ToDisplayString()}]",
                        types.BindToType.ToDisplayString()
                        );
                }

                types = new BindingContainerTypes(
                    bindFromTypeSemantics,
                    updatedBindToTypeSemantic,
                    factoryPayloadSemantic
                    );
            }

            return types;
        }

        private ITypeSymbol? GetFactoryPayloadIfExists(
            GenericNameSyntax? factoryPayloadGenericNode
            )
        {
            if (factoryPayloadGenericNode is null)
            {
                return null;
            }

            var factoryPayloadSyntax = factoryPayloadGenericNode.TypeArgumentList.Arguments.First();
            var factoryPayloadSemantic = _semanticModel.GetTypeInfo(factoryPayloadSyntax).Type;
            if (factoryPayloadSemantic == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access {nameof(factoryPayloadSemantic)}"
                    );
            }

            return factoryPayloadSemantic;
        }

        private List<ITypeSymbol> GetBindFromTypes(GenericNameSyntax bindGenericNode)
        {
            var bindFromTypeSemantics = new List<ITypeSymbol>();
            foreach (var node in bindGenericNode.TypeArgumentList.Arguments)
            {
                var bindFromTypeSemantic = _semanticModel.GetTypeInfo(node).Type;
                if (bindFromTypeSemantic == null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Unknown problem to access {nameof(bindFromTypeSemantic)}"
                        );
                }
                bindFromTypeSemantics.Add(bindFromTypeSemantic);
            }

            return bindFromTypeSemantics;
        }

        private void CheckForFromAndToTypes(
            List<ITypeSymbol> bindFromTypeSemantics,
            ITypeSymbol bindToTypeSemantic,
            bool toFactory
            )
        {
            if (bindFromTypeSemantics is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypeSemantics));
            }

            if (bindToTypeSemantic is null)
            {
                throw new ArgumentNullException(nameof(bindToTypeSemantic));
            }

            //check for target type correct
            if (bindToTypeSemantic.TypeKind.NotIn(TypeKind.Class, TypeKind.Struct))
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectTarget,
                    $"Type [{bindToTypeSemantic.ToDisplayString()}] is not a class or struct",
                    bindToTypeSemantic.ToDisplayString()
                    );
            }

            if (!toFactory)
            {
                //it's not a factory

                //check for cast exists
                foreach (var bindFromSemantic in bindFromTypeSemantics)
                {
                    if (!bindToTypeSemantic.CanBeCastedTo(bindFromSemantic.ToDisplayString()))
                    {
                        throw new DpdtException(
                            DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom,
                            $"Type [{bindToTypeSemantic.ToDisplayString()}] cannot be casted to [{bindFromSemantic.ToDisplayString()}]",
                            bindToTypeSemantic.ToDisplayString()
                            );
                    }
                }
            }
            else
            {
                //it's a factory
                if(bindFromTypeSemantics.Count > 1)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectFrom,
                        $"It is allowed only from bind from type for factory bindings. Take a look to [{bindToTypeSemantic.ToDisplayString()}] binding.",
                        bindToTypeSemantic.ToDisplayString()
                        );
                }

                //check for partial clause
                if (bindToTypeSemantic.DeclaringSyntaxReferences.Length > 1)
                {
                    //it's partial!
                }
                else
                {
                    var syntax = bindToTypeSemantic.DeclaringSyntaxReferences[0].GetSyntax();
                    if(syntax is ClassDeclarationSyntax cds)
                    {
                        if(!cds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
                        {
                            throw new DpdtException(
                                DpdtExceptionTypeEnum.TargetClassMustBePartial,
                                $"Type [{bindToTypeSemantic.ToDisplayString()}] must be partial",
                                bindToTypeSemantic.ToDisplayString()
                                );
                        }
                    }
                }
            }
        }

        private ArgumentSyntax? DetermineArgumentSubClause(
            ExpressionStatementSyntax expressionNode,
            string identifierName
            )
        {
            if (expressionNode is null)
            {
                throw new ArgumentNullException(nameof(expressionNode));
            }

            if (identifierName is null)
            {
                throw new ArgumentNullException(nameof(identifierName));
            }

            var ednodes = expressionNode
                .DescendantNodes()
                .ToList()
                ;

            var index = ednodes.FindIndex(n =>
                n is IdentifierNameSyntax ins
                && ins.Identifier.Text == identifierName
                );

            if (index < 0)
            {
                return null;
            }
            if (ednodes.Count == index + 1)
            {
                return null;
            }

            var ai = ednodes[index + 1];

            if (ai is not ArgumentListSyntax als)
            {
                return null;
            }

            var argument0 = als.Arguments[0];

            return argument0;
        }

        private BindScopeEnum DetermineScope(
            ExpressionStatementSyntax expressionNode
            )
        {
            var dnodes = expressionNode
                .DescendantNodes()
                .ToList()
                ;

            var singletonScope = dnodes.OfType<IdentifierNameSyntax>().Any(j => j.Identifier.Text == nameof(IScopeBinding.WithSingletonScope));
            if (singletonScope)
            {
                return BindScopeEnum.Singleton;
            }

            var transientScope = dnodes.OfType<IdentifierNameSyntax>().Any(j => j.Identifier.Text == nameof(IScopeBinding.WithTransientScope));
            if (transientScope)
            {
                return BindScopeEnum.Transient;
            }

            var constScope = dnodes.OfType<IdentifierNameSyntax>().Any(j => j.Identifier.Text == nameof(IToOrConstantBinding.WithConstScope));
            if (constScope)
            {
                return BindScopeEnum.Constant;
            }

            var customScope = dnodes.OfType<IdentifierNameSyntax>().Any(j => j.Identifier.Text == nameof(IScopeBinding.WithCustomScope));
            if (customScope)
            {
                return BindScopeEnum.Custom;
            }

            throw new InvalidOperationException("unknown scope");
        }


        private bool ToMethodPredicate(
            GenericNameSyntax n
            )
        {
            if (n is null)
            {
                return false;
            }

            if (n.Identifier.Text.NotIn(nameof(IToOrConstantBinding.To), nameof(IToOrConstantBinding.ToIsolatedFactory)))
            {
                return false;
            }

            var symbol = _semanticModel.GetSymbolInfo(n).Symbol;

            if (symbol?.ContainingType is null)
            {
                return false;
            }

            if (symbol.ContainingType.ToDisplayString().NotIn(
                    typeof(IToOrConstantBinding).FullName,
                    typeof(IToOrConstantBinding).FullName)
                )
            {
                return false;
            }

            return true;
        }

    }
}
