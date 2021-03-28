﻿using System;
using System.Collections.Generic;
using DpdtInject.Generator.Core.TypeInfo;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Injector.Excp;
using System.Linq;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using DpdtInject.Generator.Core.Helpers;
using System.Collections.Immutable;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory
{
    public class ConventionalBindExpressionFactory : BindExpressionFactory
    {
        private readonly ITypeInfoContainer _typeInfoContainer;
        private readonly ConstructorArgumentFromSyntaxExtractor _extractor;
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;

        public ConventionalBindExpressionFactory(
            ITypeInfoContainer typeInfoContainer,
            ConstructorArgumentFromSyntaxExtractor extractor,
            ConstructorArgumentDetector constructorArgumentDetector
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

            _typeInfoContainer = typeInfoContainer;
            _extractor = extractor;
            _constructorArgumentDetector = constructorArgumentDetector;
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

            var scanInList = invocationSymbols
                .Where(
                    s => s.Item2.ContainingType.ToDisplayString() == typeof(DefaultCluster).FullName && s.Item2.Name == DefaultCluster.ScanInAssembliesWithMethodName
                    )
                .SelectMany(s => s.Item2.TypeArguments)
                .ToList()
                ;

            var selectWithSet = invocationSymbols
                .Where(
                    s => s.Item2.ContainingType.ToDisplayString().In(typeof(IConventionalBinding).FullName, typeof(IConventionalBinding2).FullName) && s.Item2.Name == nameof(IConventionalBinding.SelectAllWith)
                    )
                .SelectMany(s => s.Item2.TypeArguments)
                .ToList()
                ;

            var excludeWithSet = invocationSymbols
                .Where(
                    s => s.Item2.ContainingType.ToDisplayString().In(typeof(IConventionalBinding).FullName, typeof(IConventionalBinding2).FullName) && s.Item2.Name == nameof(IConventionalBinding2.ExcludeAllWith)
                    )
                .SelectMany(s => s.Item2.TypeArguments)
                .ToList()
                ;

            var assemblies = new HashSet<IAssemblySymbol>(SymbolEqualityComparer.Default);
            foreach (var type in scanInList)
            {
                assemblies.Add(type.ContainingAssembly);
            }

            var processed = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GlobalNamespace.GetAllTypes(t => t.TypeKind.In(TypeKind.Struct, TypeKind.Class) && !t.IsAbstract))
                {
                    if (processed.Contains(type))
                    {
                        continue;
                    }

                    if (excludeWithSet.Any(ew => type.CanBeCastedTo(ew.GetFullyQualifiedName())))
                    {
                        continue;
                    }

                    if (selectWithSet.All(sw => !type.CanBeCastedTo(sw.GetFullyQualifiedName())))
                    {
                        continue;
                    }

                    var inters = type.AllInterfaces.Cast<ITypeSymbol>().ToImmutableArray();

                    result.Add(
                        new STCParsedBindExpression(
                            _typeInfoContainer,
                            _extractor,
                            _constructorArgumentDetector,
                            null,
                            invocationSymbols,
                            inters,
                            type,
                            scope
                            )
                        );

                    processed.Add(type);
                }
            }

            return result;
        }
    }

    //public enum ConventionalBindingModeEnum
    //{
    //    ToConcreteType,
    //    ToBaseClass,
    //    ToAllInterfaces
    //}
}
