using System;
using System.Collections.Generic;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.Conventional;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory
{
    public class ConventionalBindExpressionFactory : BindExpressionFactory
    {
        private readonly ITypeInfoContainer _typeInfoContainer;
        private readonly ConstructorArgumentFromSyntaxExtractor _extractor;
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;
        private readonly IDiagnosticReporter _diagnosticReporter;

        public ConventionalBindExpressionFactory(
            ITypeInfoContainer typeInfoContainer,
            ConstructorArgumentFromSyntaxExtractor extractor,
            ConstructorArgumentDetector constructorArgumentDetector,
            IDiagnosticReporter diagnosticReporter
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

            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            _typeInfoContainer = typeInfoContainer;
            _extractor = extractor;
            _constructorArgumentDetector = constructorArgumentDetector;
            _diagnosticReporter = diagnosticReporter;
        }

        public override IReadOnlyList<IParsedBindExpression> Create(
            ExpressionStatementSyntax expressionNode,
            List<Tuple<InvocationExpressionSyntax, IMethodSymbol>> invocationSymbols
            )
        {
            if (expressionNode is null)
            {
                throw new ArgumentNullException(nameof(expressionNode));
            }

            if (invocationSymbols is null)
            {
                throw new ArgumentNullException(nameof(invocationSymbols));
            }

            var result = new List<IParsedBindExpression>();

            var scope = DetermineScope(invocationSymbols);
            if (scope == BindScopeEnum.Constant)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    "Conventional bindings does not support constant scope."
                    );
            }

            var bindingSyntaxParser = new ConventionalBindingSyntaxParser(
                expressionNode,
                invocationSymbols
                );

            var assemblies = bindingSyntaxParser.GetAssemblesOfInterest();

            var processed = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GlobalNamespace.GetAllTypes(t => t.TypeKind.In(TypeKind.Struct, TypeKind.Class) && !t.IsAbstract))
                {
                    if (processed.Contains(type))
                    {
                        continue;
                    }

                    if (bindingSyntaxParser.ExcludeWithSet.Any(ew => type.CanBeCastedTo(ew)))
                    {
                        _diagnosticReporter.ReportInfo(
                            $"Dpdt generator skipped [{type.ToFullDisplayString()}]",
                            $"Dpdt generator skipped [{type.ToFullDisplayString()}] for conventional binding due to it's excluded"
                            );
                        continue;
                    }

                    if (bindingSyntaxParser.SelectWithSet.All(sw => !type.CanBeCastedTo(sw)))
                    {
                        continue;
                    }

                    var froms = bindingSyntaxParser.FromTypesProvider.GetBindFromTypes(type);
                    var to = GetTo(invocationSymbols, type);

                    result.Add(
                        new STCParsedBindExpression(
                            _typeInfoContainer,
                            _extractor,
                            _constructorArgumentDetector,
                            expressionNode,
                            invocationSymbols,
                            froms,
                            to,
                            scope,
                            true
                            )
                        );

                    _diagnosticReporter.ReportInfo(
                        $"Dpdt generator processed [{type.ToFullDisplayString()}]",
                        $"Dpdt generator processed [{type.ToFullDisplayString()}] for conventional binding"
                        );

                    processed.Add(type);
                }
            }

            return result;
        }


    }
}
