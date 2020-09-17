using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Generator.Helpers
{
    public static class RoslynHelper
    {
        //public static SyntaxReference GetOnlyMethod(
        //    this INamedTypeSymbol classSymbol,
        //    string methodName
        //    )
        //{
        //    IMethodSymbol foundMethodSymbol = null;
        //    while (classSymbol != null)
        //    {
        //        var members = classSymbol.GetMembers(methodName);

        //        if (members.Length > 1)
        //        {
        //            throw new Exception($"Something wrong with type {classSymbol.GetFullName()}");
        //        }

        //        if (members.Length == 0)
        //        {
        //            classSymbol = classSymbol.BaseType;
        //            continue;
        //        }

        //        foundMethodSymbol = (IMethodSymbol)members[0];
        //        break;
        //    }

        //    if(foundMethodSymbol == null)
        //    {
        //        throw new Exception($"Not found {methodName}");
        //    }

        //    var refs = foundMethodSymbol.DeclaringSyntaxReferences;

        //    if (refs.Length != 1)
        //    {
        //        throw new Exception($"Something wrong with method {foundMethodSymbol.GetFullName()}");
        //    }

        //    var methodRef = refs[0];

        //    return methodRef;
        //}

        [return: MaybeNull]
        public static T Root<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            var processed = node;
            while (processed != null)
            {
                if (processed is T t)
                {
                    return t;
                }

                processed = processed.Parent;
            }

            return null;
        }

        public static IEnumerable<INamedTypeSymbol> GetAllTypes(this INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
                foreach (var nestedType in type.GetNestedTypes())
                    yield return nestedType;

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
                foreach (var type in nestedNamespace.GetAllTypes())
                    yield return type;
        }

        public static IEnumerable<INamedTypeSymbol> GetNestedTypes(this INamedTypeSymbol type)
        {
            yield return type;
            foreach (var nestedType in type.GetTypeMembers()
                .SelectMany(nestedType => nestedType.GetNestedTypes()))
                yield return nestedType;
        }

        public static string GetFullName(
            this ISymbol symbol
            )
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            return symbol.ContainingNamespace + "." + symbol.Name;
        }

        public static bool TryGetCompileTimeString(
            this ExpressionSyntax expression,
            SemanticModel semanticModel,
            [NotNullWhen(true)] out string? result
            )
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (expression is LiteralExpressionSyntax literal)
            {
                result = literal.Token.ValueText;
                return true;
            }

            var constant = semanticModel.GetConstantValue(expression);
            if (constant.HasValue)
            {
                result = constant.Value.ToString()!;
                return true;
            }

            result = null;
            return false;
        }

        public static bool CanBeCastedTo(
            this ITypeSymbol target,
            string subjectTypeFullName
            )
        {
            var roslynTypeFullName = target.ContainingNamespace.Name + "." + target.Name;

            if (StringComparer.InvariantCultureIgnoreCase.Compare(roslynTypeFullName, subjectTypeFullName) == 0)
            {
                return true;
            }

            foreach (INamedTypeSymbol @interface in target.AllInterfaces)
            {
                var roslynInterfaceFullName = @interface.GetFullName();

                if (StringComparer.InvariantCultureIgnoreCase.Compare(roslynInterfaceFullName, subjectTypeFullName) == 0)
                {
                    return true;
                }
            }

            if (target.BaseType != null && !SymbolEqualityComparer.Default.Equals(target.BaseType, target))
            {
                if (CanBeCastedTo(target.BaseType, subjectTypeFullName))
                {
                    return true;
                }
            }

            foreach (INamedTypeSymbol @interface in target.AllInterfaces)
            {
                var r = CanBeCastedTo(
                    @interface,
                    subjectTypeFullName
                    );

                if (r)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
