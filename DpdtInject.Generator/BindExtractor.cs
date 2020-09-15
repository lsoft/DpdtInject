using DpdtInject.Generator.Blocks.Binding;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
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

namespace DpdtInject.Generator
{
    public class BindExtractor : CSharpSyntaxRewriter
    {
        public static readonly string ComplexSeparator = "," + Environment.NewLine;

        private readonly Compilation _compilation;
        private readonly CompilationUnitSyntax _compilationUnitSyntax;
        private readonly SemanticModel _semanticModel;

        private readonly List<BindingProcessor> _bindingProcessors;

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

            _bindingProcessors = new List<BindingProcessor>();
        }


        public BindingProcessorContainer GetBindingProcessors()
        {
            return new BindingProcessorContainer(_bindingProcessors);
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

            if (genericNodes.Count < 2)
            {
                return base.VisitExpressionStatement(expressionNode)!;
            }

            var bindGenericNode = genericNodes[0];
            var bindMethodName = bindGenericNode.Identifier.Text;
            if (bindMethodName != "Bind")
            {
                return base.VisitExpressionStatement(expressionNode)!;
            }

            var toGenericNode = genericNodes[1];
            var toMethodName = toGenericNode.Identifier.Text;
            if (toMethodName != "To")
            {
                return base.VisitExpressionStatement(expressionNode)!;
            }

            //looks like we found what we want

            var bindFromTypeName = string.Join("_", bindGenericNode.TypeArgumentList.Arguments);

            var bindToTypeName = toGenericNode.TypeArgumentList.ToFullString();
            bindToTypeName = bindToTypeName.Substring(1, bindToTypeName.Length - 2);

            var suffixName = string.Empty;// "_" + Guid.NewGuid().ToString().Replace("-", "");
            var nodeVariableName = "node" + suffixName;
            var containerVariableName = "container" + suffixName;

            //extract constructor argument names

            var scope = DetermineScope(expressionNode);

            StatementSyntax toReplace0Node, toReplace1Node;
            switch (scope)
            {
                case BindScopeEnum.Singleton:
                    ProcessSingleton(
                        expressionNode,
                        bindGenericNode,
                        toGenericNode,
                        bindFromTypeName,
                        bindToTypeName,
                        nodeVariableName,
                        containerVariableName
                        //out toReplace0Node,
                        //out toReplace1Node
                        );
                    break;
                //case BindScopeEnum.Transient:
                //    ProcessTransient(
                //        expressionNode,
                //        toGenericNode,
                //        bindFromTypeName,
                //        bindToTypeName,
                //        nodeVariableName,
                //        containerVariableName,
                //        out toReplace0Node,
                //        out toReplace1Node
                //        );
                //    break;
                //case BindScopeEnum.Constant:
                //    ProcessConstant(
                //        expressionNode,
                //        bindFromTypeName,
                //        nodeVariableName,
                //        containerVariableName,
                //        out toReplace0Node,
                //        out toReplace1Node
                //        );
                //    break;
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

        private void ProcessSingleton(
            ExpressionStatementSyntax expressionNode,
            GenericNameSyntax bindGenericNode,
            GenericNameSyntax toGenericNode,
            string bindFromTypeName,
            string bindToTypeName,
            string nodeVariableName,
            string containerVariableName
            //out StatementSyntax toReplace0Node,
            //out StatementSyntax toReplace1Node
            )
        {
            var caExtractor = new ConstructorArgumentExtractor(
                _compilation,
                _semanticModel
                );
            caExtractor.Visit(expressionNode);

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


            var bindToTypeSematic = _semanticModel.GetTypeInfo(toGenericNode.TypeArgumentList.DescendantNodes().First()).Type;
            if (bindToTypeSematic == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access {nameof(bindToTypeSematic)}"
                    );
            }

            var fullBindToTypeName = _compilation.GetTypeByMetadataName(bindToTypeSematic.GetFullName());
            if (fullBindToTypeName == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Unknown problem to access type for {bindToTypeSematic.GetFullName()}"
                    );
            }

            var constructorArguments = caExtractor.GetConstructorArguments();

            var chosenConstructor = ChooseConstructor(
                fullBindToTypeName,
                constructorArguments
                );

            foreach (var cParameter in chosenConstructor.Parameters)
            {
                var cParameterName = cParameter.Name;
                var cParameterType = cParameter.Type;

                var found = constructorArguments.FirstOrDefault(ca => ca.Name == cParameterName);
                if(found is null)
                {
                    constructorArguments.Add(
                        new DetectedConstructorArgument(
                            cParameterName,
                            cParameterType
                            )
                        );
                }
            }


            var bindingProcessor = new BindingProcessor(
                bindFromTypeSematics,
                bindToTypeSematic,
                constructorArguments,
                BindScopeEnum.Singleton
                );

            _bindingProcessors.Add(bindingProcessor);


//            var createRequestStatements = new List<string>();
//            var castStatements = new List<string>();
//            var index = 0;
//            foreach (var cParameter in chosenConstructor.Parameters)
//            {
//                var cParameterName = cParameter.Name;
//                var cParameterType = cParameter.Type;

//                var createRequestStatement = $@"new {nameof(Request)}(typeof({cParameterType}), namedContainer.{nameof(NamedInstancesContainer.Name)}, ""{cParameterName}"")";

//                createRequestStatements.Add(createRequestStatement);

//                castStatements.Add($@"({cParameterType})arguments[{index}]");

//                index++;
//            }

//            var getDependencyRequestsBody = new StringBuilder();
//            if (createRequestStatements.Count > 0)
//            {
//                getDependencyRequestsBody.AppendLine($"var result =  new {nameof(Request)}[{createRequestStatements.Count}]");
//                getDependencyRequestsBody.AppendLine("{");

//                foreach (var pair in createRequestStatements)
//                {
//                    getDependencyRequestsBody.AppendLine(
//                        pair + ","
//                        );
//                }

//                getDependencyRequestsBody.AppendLine("};");
//                getDependencyRequestsBody.AppendLine($"return result;");
//            }
//            else
//            {
//                getDependencyRequestsBody.AppendLine($"return new {nameof(Request)}[0];");
//            }


//            var containerClassName = $"{nameof(PregeneratedSingletonInstanceContainer)}_{bindFromTypeName}_{Guid.NewGuid().ToString().ConvertMinusToGround()}";
//            var containerClass = @$"
//private class {containerClassName} : {typeof(PregeneratedSingletonInstanceContainer).FullName}
//{{
//    public {containerClassName}(
//        {nameof(BindConfiguration)} configuration
//        ) : base(configuration)
//    {{
//    }}

//    protected override object Instanciate(
//        object[] arguments
//        )
//    {{
//        System.Diagnostics.Debug.WriteLine($""DPDT: Perform pregenerated creation of {fullBindToTypeName}"");

//        var instance = new {bindToTypeName}(
//            {string.Join(ComplexSeparator, castStatements)}
//            );

//        return instance;
//    }}

//    public override {nameof(Request)}[] {nameof(BaseInstanceContainer.GetDependencyRequests)}(
//        {nameof(NamedInstancesContainer)} namedContainer
//        )
//    {{
//        {getDependencyRequestsBody.ToString()}
//    }}

//}}
//";

//            ContainerClassDeclarations.Add(containerClass);

//            //transform it

//            toReplace0Node = SyntaxFactory.ParseStatement(
//                $"var {nodeVariableName} = " + expressionNode.WithoutLeadingTrivia().GetText().ToString()
//                ).WithLeadingTrivia(expressionNode.GetLeadingTrivia());

//            toReplace1Node = SyntaxFactory.ParseStatement($@"{Environment.NewLine}var {containerVariableName} = new {containerClassName}({nodeVariableName}.{nameof(DefineBindingNode.CreateConfiguration)}());
//                containers.Add({containerVariableName}.Configuration.BindNode.Name, {containerVariableName});
//");

        }

        private BindScopeEnum DetermineScope(
            ExpressionStatementSyntax expressionNode
            )
        {
            var dnodes = expressionNode
                .DescendantNodes()
                .ToList()
                ;

            var singletonScope = dnodes.OfType<IdentifierNameSyntax>().Any(j => j.Identifier.Text == nameof(DefineBindingNode.WithSingletonScope));
            if (singletonScope)
            {
                return BindScopeEnum.Singleton;
            }

            //var transientScope = dnodes.OfType<IdentifierNameSyntax>().Any(j => j.Identifier.Text == nameof(DefineBindingNode.WithTransientScope));
            //if (transientScope)
            //{
            //    return BindScopeEnum.Transient;
            //}
            
            //var constScope = dnodes.OfType<IdentifierNameSyntax>().Any(j => j.Identifier.Text == nameof(DefineBindingNode.WithConstScope));
            //if(constScope)
            //{
            //    return BindScopeEnum.Constant;
            //}

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
            foreach (var constructor in fullBindToTypeName.Constructors)
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
            if(constructorArguments.Count == 0)
            {
                return true;
            }

            foreach(var ca in constructorArguments)
            {
                var caName = ca.Name;

                if(!constructor.Parameters.Any(j => j.Name == caName))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class BindingProcessorContainer
    {
        private readonly List<BindingProcessor> _bindingProcessors;

        public IReadOnlyList<BindingProcessor> BindingProcessors => _bindingProcessors;

        public BindingProcessorContainer(
            List<BindingProcessor> bindingProcessors
            )
        {
            if (bindingProcessors is null)
            {
                throw new ArgumentNullException(nameof(bindingProcessors));
            }
            _bindingProcessors = bindingProcessors;
        }

        public IReadOnlyList<BindingProcessor> GetBindWith(
            string bindFromTypeFullName
            )
        {
            if (bindFromTypeFullName is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypeFullName));
            }

            return
                _bindingProcessors.FindAll(bc => bc.FromTypeFullNames.Contains(bindFromTypeFullName));
        }

        public BindingProcessorGroups ConvertToGroups()
        {
            return new BindingProcessorGroups(this._bindingProcessors);
        }

        internal void AnalyzeForCircularDependencies(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            new CycleChecker(ConvertToGroups())
                .CheckForCycles(diagnosticReporter)
                ;
        }
    }

    public class BindingProcessorGroups
    {
        public List<BindingProcessor> BindingProcessors
        {
            get;
        }

        public Dictionary<ITypeSymbol, List<BindingProcessor>> ProcessorGroups
        {
            get;
        }

        public BindingProcessorGroups(
            List<BindingProcessor> bindingProcessors
            )
        {
            if (bindingProcessors is null)
            {
                throw new ArgumentNullException(nameof(bindingProcessors));
            }

            var processorGroups = new Dictionary<ITypeSymbol, List<BindingProcessor>>(
                new TypeSymbolEqualityComparer()
                );

            foreach (var bindingProcessor in bindingProcessors)
            {
                foreach (var bindFromType in bindingProcessor.BindFromTypes)
                {
                    if (!processorGroups.ContainsKey(bindFromType))
                    {
                        processorGroups[bindFromType] = new List<BindingProcessor>();
                    }

                    processorGroups[bindFromType].Add(bindingProcessor);
                }
            }

            ProcessorGroups = processorGroups;
            BindingProcessors = bindingProcessors;
        }

    }

    public class TypeSymbolEqualityComparer : IEqualityComparer<ITypeSymbol>
    {
        public bool Equals([AllowNull] ITypeSymbol x, [AllowNull] ITypeSymbol y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null)
            {
                return false;
            }
            if (y is null)
            {
                return false;
            }

            if (x.GetFullName() == y.GetFullName())
            {
                return true;
            }

            return false;
        }

        public int GetHashCode([DisallowNull] ITypeSymbol obj)
        {
            return obj.GetFullName().GetHashCode();
        }
    }

    public class CycleChecker
    {
        private readonly BindingProcessorGroups _groups;

        public CycleChecker(
            BindingProcessorGroups groups
            )
        {
            _groups = groups;
        }

        public void CheckForCycles(
            IDiagnosticReporter reporter
            )
        {
            if (reporter is null)
            {
                throw new ArgumentNullException(nameof(reporter));
            }

            var cycles = new HashSet<CycleFoundException>(
                new OrderIndependentCycleFoundEqualityComparer()
                );

            foreach(var bindFromType in _groups.ProcessorGroups.Keys.Shuffle())
            {
                try
                {
                    var used = new Subgraph();

                    CheckForCyclesInternal(
                        ref used,
                        bindFromType
                        );
                }
                catch(CycleFoundException cfe)
                {
                    if (!cycles.Contains(cfe))
                    {
                        cycles.Add(cfe);
                    }
                }
            }

            foreach(var cycle in cycles)
            {
                if (cycle.StrictConculsion)
                {
                    reporter.ReportError(
                        $"A circular dependency was found",
                        $"A circular dependency was found: [{cycle.GetStringRepresentation()}]"
                        );
                }
                else
                {
                    reporter.ReportWarning(
                        $"Perhaps a circular dependency was found",
                        $"Perhaps a circular dependency was found, please take a look: [{cycle.GetStringRepresentation()}]"
                        );
                }
            }
        }

        private void CheckForCyclesInternal(
            ref Subgraph used,
            ITypeSymbol requestedType
            )
        {
            foreach (var bindingProcessor in _groups.ProcessorGroups[requestedType])
            {
                var used2 = used.Clone();

                used2.AppendOrFailIfExists(
                    requestedType,
                    !bindingProcessor.IsConditional
                    );

                foreach (var constructorArgument in bindingProcessor.ConstructorArguments.Where(ca => !ca.DefineInBindNode).Shuffle())
                {
                    if(constructorArgument.Type is null)
                    {
                        throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"constructorArgument.Type is null somehow");
                    }

                    CheckForCyclesInternal(
                        ref used2,
                        constructorArgument.Type
                        );
                }
            }
        }

        private class OrderIndependentCycleFoundEqualityComparer : IEqualityComparer<CycleFoundException>
        {
            public bool Equals([AllowNull] CycleFoundException x, [AllowNull] CycleFoundException y)
            {
                if(ReferenceEquals(x,y))
                {
                    return true;
                }

                if(x is null)
                {
                    return false;
                }
                if(y is null)
                {
                    return false;
                }

                return this.GetHashCode(x) == this.GetHashCode(y);
            }

            public int GetHashCode([DisallowNull] CycleFoundException obj)
            {
                var result = obj.StrictConculsion ? int.MaxValue : 0;

                foreach(var type in obj.CycleList.Skip(1))
                {
                    result ^= type.GetFullName().GetHashCode();
                }

                return result;
            }
        }

    }

    public class Subgraph
    {
        private readonly HashSet<ITypeSymbol> _used;
        private readonly List<ITypeSymbol> _usedInList;

        private bool _idempotent = true;

        public Subgraph()
        {
            _used = new HashSet<ITypeSymbol>(
                new TypeSymbolEqualityComparer()
                );
            _usedInList = new List<ITypeSymbol>();
        }

        private Subgraph(Subgraph subgraph)
        {
            _used = new HashSet<ITypeSymbol>(subgraph._used, subgraph._used.Comparer);
            _usedInList = new List<ITypeSymbol>(subgraph._usedInList);
            _idempotent = subgraph._idempotent;
        }

        public void AppendOrFailIfExists(
            ITypeSymbol node,
            bool idempotent
            )
        {
            if (_used.Contains(node))
            {
                var cycleList = new List<ITypeSymbol>(_usedInList);
                cycleList.Add(node);

                throw new CycleFoundException(
                    cycleList,
                    _idempotent
                    );
            }

            _used.Add(node);
            _usedInList.Add(node);
            _idempotent &= idempotent;
        }

        public Subgraph Clone()
        {
            return new Subgraph(
                this
                );
        }
    }
}
