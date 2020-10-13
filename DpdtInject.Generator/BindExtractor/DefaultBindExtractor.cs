using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
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

namespace DpdtInject.Generator.BindExtractor
{
    public class DefaultBindExtractor : CSharpSyntaxRewriter
    {
        public static readonly string ComplexSeparator = "," + Environment.NewLine;

        private readonly ITypeInfoProvider _typeInfoProvider;
        private readonly CompilationUnitSyntax _compilationUnitSyntax;
        private readonly SemanticModel _semanticModel;
        private readonly List<IBindingContainer> _bindingContainers;

        public DefaultBindExtractor(
            ITypeInfoProvider typeInfoProvider,
            CompilationUnitSyntax compilationUnitSyntax
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            if (compilationUnitSyntax is null)
            {
                throw new ArgumentNullException(nameof(compilationUnitSyntax));
            }

            _typeInfoProvider = typeInfoProvider;
            _compilationUnitSyntax = compilationUnitSyntax;
            _semanticModel = typeInfoProvider.GetSemanticModel(compilationUnitSyntax.SyntaxTree);

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

            var toGenericNode = genericNodes[1];
            var toMethodName = toGenericNode.Identifier.Text;
            if (toMethodName.NotIn( nameof(IToOrConstantBinding.To), nameof(IToOrConstantBinding.ToFactory) ))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, "Cannot find To clause for singleton binding");
            }

            var toFactory = toMethodName == nameof(IToOrConstantBinding.ToFactory);

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
                toFactory
                );

            var fullBindToTypeName = _typeInfoProvider.GetTypeByMetadataName(bindToTypeSemantic.ToDisplayString());
            if (fullBindToTypeName == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access type for {bindToTypeSemantic.ToDisplayString()}"
                    );
            }

            var caExtractor = new ConstructorArgumentExtractor(
                _typeInfoProvider,
                _semanticModel
                );
            caExtractor.Visit(expressionNode);

            var constructorArguments = GetConstructorArguments(
                caExtractor,
                fullBindToTypeName
                );

            var bindingContainer = new BindingContainerWithInstance(
                bindFromTypeSemantics,
                bindToTypeSemantic,
                constructorArguments,
                BindScopeEnum.Singleton,
                whenArgumentClause,
                toFactory
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

            var toGenericNode = genericNodes[1];
            var toMethodName = toGenericNode.Identifier.Text;
            if (toMethodName.NotIn(nameof(IToOrConstantBinding.To), nameof(IToOrConstantBinding.ToFactory)))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, "Cannot find To clause for transient binding");
            }

            var toFactory = toMethodName == nameof(IToOrConstantBinding.ToFactory);

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
                toFactory
                );

            var fullBindToTypeName = _typeInfoProvider.GetTypeByMetadataName(bindToTypeSemantic.ToDisplayString());
            if (fullBindToTypeName == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access type for {bindToTypeSemantic.ToDisplayString()}"
                    );
            }

            var caExtractor = new ConstructorArgumentExtractor(
                _typeInfoProvider,
                _semanticModel
                );
            caExtractor.Visit(expressionNode);

            var constructorArguments = GetConstructorArguments(
                caExtractor,
                fullBindToTypeName
                );

            var bindingContainer = new BindingContainerWithInstance(
                bindFromTypeSemantics,
                bindToTypeSemantic,
                constructorArguments,
                BindScopeEnum.Transient,
                whenArgumentClause,
                toFactory
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

            var toGenericNode = genericNodes[1];
            var toMethodName = toGenericNode.Identifier.Text;
            if (toMethodName.NotIn(nameof(IToOrConstantBinding.To), nameof(IToOrConstantBinding.ToFactory)))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, "Cannot find To clause for custom binding");
            }

            var toFactory = toMethodName == nameof(IToOrConstantBinding.ToFactory);

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
                toFactory
                );

            var fullBindToTypeName = _typeInfoProvider.GetTypeByMetadataName(bindToTypeSemantic.ToDisplayString());
            if (fullBindToTypeName == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access type for {bindToTypeSemantic.ToDisplayString()}"
                    );
            }

            var caExtractor = new ConstructorArgumentExtractor(
                _typeInfoProvider,
                _semanticModel
                );
            caExtractor.Visit(expressionNode);

            var constructorArguments = GetConstructorArguments(
                caExtractor,
                fullBindToTypeName
                );

            var bindingContainer = new BindingContainerWithInstance(
                bindFromTypeSemantics,
                bindToTypeSemantic,
                constructorArguments,
                BindScopeEnum.Custom,
                whenArgumentClause,
                toFactory
                );

            _bindingContainers.Add(bindingContainer);
        }

        private void ProcessConstant(
            ExpressionStatementSyntax expressionNode,
            GenericNameSyntax bindGenericNode,
            ArgumentSyntax? whenArgumentClause
            )
        {
            var genericNodes = expressionNode
                .DescendantNodes()
                .OfType<GenericNameSyntax>()
                .ToList();


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

            var bindingContainer = new ConstantBindingContainer(
                bindFromTypeSemantics,
                constTypeSymbol,
                constantClause,
                BindScopeEnum.Constant,
                whenArgumentClause
                );

            _bindingContainers.Add(bindingContainer);
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

        private List<DetectedConstructorArgument> GetConstructorArguments(
            ConstructorArgumentExtractor extractor,
            INamedTypeSymbol fullBindToTypeName
            )
        {
            if (extractor is null)
            {
                throw new ArgumentNullException(nameof(extractor));
            }

            if (fullBindToTypeName is null)
            {
                throw new ArgumentNullException(nameof(fullBindToTypeName));
            }

            var constructorArguments = extractor.GetConstructorArguments();

            var chosenConstructor = ChooseConstructor(
                fullBindToTypeName,
                constructorArguments
                );


            foreach (var cParameter in chosenConstructor.Parameters)
            {
                var cParameterName = cParameter.Name;
                var cParameterType = cParameter.Type;

                var found = constructorArguments.FirstOrDefault(ca => ca.Name == cParameterName);
                if (found is null)
                {
                    constructorArguments.Add(
                        new DetectedConstructorArgument(
                            cParameterName,
                            cParameterType,
                            cParameter.HasExplicitDefaultValue
                            )
                        );
                }
            }

            return constructorArguments;
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
                //check for cast exists
                foreach (var bindFromSemantic in bindFromTypeSemantics)
                {
                    if (!bindToTypeSemantic.CanBeCastedTo(bindFromSemantic.ToDisplayString()))
                    {
                        throw new DpdtException(
                            DpdtExceptionTypeEnum.IncorrectBinding_CantCast,
                            $"Type [{bindToTypeSemantic.ToDisplayString()}] cannot be casted to [{bindFromSemantic.ToDisplayString()}]",
                            bindToTypeSemantic.ToDisplayString()
                            );
                    }
                }
            }
            else
            {
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

        private IMethodSymbol ChooseConstructor(
            INamedTypeSymbol fullBindToTypeName,
            IReadOnlyList<DetectedConstructorArgument> constructorArguments
            )
        {
            if (fullBindToTypeName is null)
            {
                throw new ArgumentNullException(nameof(fullBindToTypeName));
            }

            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }

            //constructor argument names exists
            //we should choose appropriate constructor
            IMethodSymbol chosenConstructor = null!;
            foreach (var constructor in fullBindToTypeName.InstanceConstructors)
            {
                if (!ContainsAllArguments(constructor, constructorArguments))
                {
                    continue;
                }

                if (chosenConstructor == null)
                {
                    chosenConstructor = constructor;
                }
                else
                {
                    if (chosenConstructor.Parameters.Length > constructor.Parameters.Length)
                    {
                        //here is some kind of hardcoded heuristic: we prefer constructor with fewer parameters
                        chosenConstructor = constructor;
                    }
                }
            }

            if (chosenConstructor == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.ConstructorArgumentMiss,
                    $@"Type {fullBindToTypeName.Name} does not contains constructor with arguments ({string.Join(",", constructorArguments)})",
                    fullBindToTypeName.Name
                    );
            }

            return chosenConstructor;
        }

        private bool ContainsAllArguments(
            IMethodSymbol constructor,
            IReadOnlyList<DetectedConstructorArgument> constructorArguments
            )
        {
            if (constructorArguments.Count == 0)
            {
                return true;
            }

            foreach (var ca in constructorArguments)
            {
                var caName = ca.Name;

                if (!constructor.Parameters.Any(j => j.Name == caName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
