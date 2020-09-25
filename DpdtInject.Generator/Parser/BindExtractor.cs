using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace DpdtInject.Generator.Parser
{
    public class BindExtractor : CSharpSyntaxRewriter
    {
        public static readonly string ComplexSeparator = "," + Environment.NewLine;

        private readonly Compilation _compilation;
        private readonly CompilationUnitSyntax _compilationUnitSyntax;
        private readonly SemanticModel _semanticModel;

        private readonly List<IBindingContainer> _bindingContainers;

        public BindExtractor(
            Compilation compilation,
            CompilationUnitSyntax compilationUnitSyntax
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (compilationUnitSyntax is null)
            {
                throw new ArgumentNullException(nameof(compilationUnitSyntax));
            }

            _compilation = compilation;
            _compilationUnitSyntax = compilationUnitSyntax;
            _semanticModel = _compilation.GetSemanticModel(compilationUnitSyntax.SyntaxTree);

            _bindingContainers = new List<IBindingContainer>();
        }


        public BindingsContainer GetBindingsContainer(
            ITypeSymbol moduleType
            )
        {
            return new BindingsContainer(
                moduleType,
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

            var declaredClusterType = DetermineCluster(genericNodes);

            if (declaredClusterType is null)
            {
                //cluster should be defined
                throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, "Cluster should be defined");
            }

            var expressionText = expressionNode.GetText().ToString();

            if (!expressionText.Contains(nameof(IToOrConstantBinding.WithConstScope)))
            {
                if (!expressionText.Contains(nameof(IScopeBinding.WithSingletonScope)))
                {
                    if (!expressionText.Contains(nameof(IScopeBinding.WithTransientScope)))
                    {
                        return base.VisitExpressionStatement(expressionNode)!;
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
                        whenArgumentClause,
                        declaredClusterType
                        );
                    break;
                case BindScopeEnum.Transient:
                    ProcessTransient(
                        expressionNode,
                        bindGenericNode,
                        whenArgumentClause,
                        declaredClusterType
                        );
                    break;
                case BindScopeEnum.Constant:
                    ProcessConstant(
                        expressionNode,
                        bindGenericNode,
                        whenArgumentClause,
                        declaredClusterType
                        );
                    break;
                default:
                    throw new DpdtException(DpdtExceptionTypeEnum.UnknownScope, $"Unknown scope {scope}");
            }

            return expressionNode;

            //var statements = new SyntaxList<StatementSyntax>();
            //statements = statements.Add(toReplace0Node);
            //statements = statements.Add(toReplace1Node);

            //var block = SyntaxFactory.Block(statements);
            //return block;
        }

        private void ProcessConstant(
            ExpressionStatementSyntax expressionNode,
            GenericNameSyntax bindGenericNode,
            ArgumentSyntax? whenArgumentClause,
            ITypeSymbol declaredClusterType
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

            var bindFromTypeSematics = new List<ITypeSymbol>();
            foreach (var node in bindGenericNode.TypeArgumentList.DescendantNodes())
            {
                var bindFromTypeSematic = _semanticModel.GetTypeInfo(node).Type;
                if (bindFromTypeSematic == null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Unknown problem to access {nameof(bindFromTypeSematic)}"
                        );
                }
                bindFromTypeSematics.Add(bindFromTypeSematic);
            }

            var bindingContainer = new ConstantBindingContainer(
                declaredClusterType,
                bindFromTypeSematics,
                constTypeSymbol,
                constantClause,
                BindScopeEnum.Constant,
                whenArgumentClause
                );

            _bindingContainers.Add(bindingContainer);
        }


        private void ProcessTransient(
            ExpressionStatementSyntax expressionNode,
            GenericNameSyntax bindGenericNode,
            ArgumentSyntax? whenArgumentClause,
            ITypeSymbol declaredClusterType
            )
        {
            var genericNodes = expressionNode
                .DescendantNodes()
                .OfType<GenericNameSyntax>()
                .ToList();

            var toGenericNode = genericNodes[1];
            var toMethodName = toGenericNode.Identifier.Text;
            if (toMethodName != nameof(IToOrConstantBinding.To))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, "Cannot find To clause for transient binding");
            }

            var bindFromTypeSematics = new List<ITypeSymbol>();
            foreach (var node in bindGenericNode.TypeArgumentList.DescendantNodes())
            {
                var bindFromTypeSematic = _semanticModel.GetTypeInfo(node).Type;
                if (bindFromTypeSematic == null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Unknown problem to access {nameof(bindFromTypeSematic)}"
                        );
                }
                bindFromTypeSematics.Add(bindFromTypeSematic);
            }

            var bindToSyntax = toGenericNode.TypeArgumentList.DescendantNodes().First();
            var bindToTypeSematic = _semanticModel.GetTypeInfo(bindToSyntax).Type;

            if (bindToTypeSematic == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access {nameof(bindToTypeSematic)}"
                    );
            }

            CheckForFromAndToTypes(
                bindFromTypeSematics,
                bindToTypeSematic
                );

            var fullBindToTypeName = _compilation.GetTypeByMetadataName(bindToTypeSematic.GetFullName());
            if (fullBindToTypeName == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access type for {bindToTypeSematic.GetFullName()}"
                    );
            }

            var caExtractor = new ConstructorArgumentExtractor(
                _compilation,
                _semanticModel
                );
            caExtractor.Visit(expressionNode);

            var constructorArguments = GetConstructorArguments(
                caExtractor,
                fullBindToTypeName
                );

            var bindingContainer = new BindingContainerWithInstance(
                declaredClusterType,
                bindFromTypeSematics,
                bindToTypeSematic,
                constructorArguments,
                BindScopeEnum.Transient,
                whenArgumentClause
                );

            _bindingContainers.Add(bindingContainer);
        }


        private void ProcessSingleton(
            ExpressionStatementSyntax expressionNode,
            GenericNameSyntax bindGenericNode,
            ArgumentSyntax? whenArgumentClause,
            ITypeSymbol declaredClusterType
            )
        {
            var genericNodes = expressionNode
                .DescendantNodes()
                .OfType<GenericNameSyntax>()
                .ToList();

            var toGenericNode = genericNodes[1];
            var toMethodName = toGenericNode.Identifier.Text;
            if (toMethodName != nameof(IToOrConstantBinding.To))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, "Cannot find To clause for singleton binding");
            }

            var bindFromTypeSematics = new List<ITypeSymbol>();
            foreach (var node in bindGenericNode.TypeArgumentList.DescendantNodes())
            {
                var bindFromTypeSematic = _semanticModel.GetTypeInfo(node).Type;
                if (bindFromTypeSematic == null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Unknown problem to access {nameof(bindFromTypeSematic)}"
                        );
                }
                bindFromTypeSematics.Add(bindFromTypeSematic);
            }

            var bindToSyntax = toGenericNode.TypeArgumentList.DescendantNodes().First();
            var bindToTypeSematic = _semanticModel.GetTypeInfo(bindToSyntax).Type;
            if (bindToTypeSematic == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access {nameof(bindToTypeSematic)}"
                    );
            }

            CheckForFromAndToTypes(
                bindFromTypeSematics,
                bindToTypeSematic
                );

            var fullBindToTypeName = _compilation.GetTypeByMetadataName(bindToTypeSematic.GetFullName());
            if (fullBindToTypeName == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access type for {bindToTypeSematic.GetFullName()}"
                    );
            }

            var caExtractor = new ConstructorArgumentExtractor(
                _compilation,
                _semanticModel
                );
            caExtractor.Visit(expressionNode);

            var constructorArguments = GetConstructorArguments(
                caExtractor,
                fullBindToTypeName
                );

            var bindingContainer = new BindingContainerWithInstance(
                declaredClusterType,
                bindFromTypeSematics,
                bindToTypeSematic,
                constructorArguments,
                BindScopeEnum.Singleton,
                whenArgumentClause
                );

            _bindingContainers.Add(bindingContainer);
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
            List<ITypeSymbol> bindFromTypeSematics,
            ITypeSymbol bindToTypeSematic
            )
        {
            if (bindFromTypeSematics is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypeSematics));
            }

            if (bindToTypeSematic is null)
            {
                throw new ArgumentNullException(nameof(bindToTypeSematic));
            }

            //check for target type correct
            if (bindToTypeSematic.TypeKind.NotIn(TypeKind.Class, TypeKind.Struct))
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectTarget,
                    $"Type [{bindToTypeSematic.GetFullName()}] is not a class or struct",
                    bindToTypeSematic.GetFullName()
                    );
            }

            //check for cast exists
            foreach (var bindFromSemantic in bindFromTypeSematics)
            {
                if (!bindToTypeSematic.CanBeCastedTo(bindFromSemantic.GetFullName()))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_CantCast,
                        $"Type [{bindToTypeSematic.GetFullName()}] cannot be casted to [{bindFromSemantic.GetFullName()}]",
                        bindToTypeSematic.GetFullName()
                        );
                }
            }
        }

        private ITypeSymbol? DetermineCluster(
            List<GenericNameSyntax> genericNodes
            )
        {
            if (genericNodes is null)
            {
                throw new ArgumentNullException(nameof(genericNodes));
            }

            ITypeSymbol? declaredClusterType = null;
            var clusterGenericNode = genericNodes.FirstOrDefault(gn => gn.Identifier.Text == nameof(IClusterBinding.InCluster));
            if (!(clusterGenericNode is null))
            {
                var clusterSyntax = clusterGenericNode.TypeArgumentList.DescendantNodes().First();
                var clusterTypeSematic = _semanticModel.GetTypeInfo(clusterSyntax).Type;

                if (clusterTypeSematic == null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Unknown problem to access {nameof(clusterTypeSematic)}"
                        );
                }

                declaredClusterType = clusterTypeSematic;
            }

            return declaredClusterType;
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
                (n is IdentifierNameSyntax ins)
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
