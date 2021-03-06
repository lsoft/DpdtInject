using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DpdtInject.Generator.BindExtractor;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;

namespace DpdtInject.Generator.Helpers
{
    public static class RoslynHelper
    {
        private static readonly SymbolDisplayFormat SymbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);


        public static string GetFullyQualifiedName(
            this ISymbol s
            )
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.ToDisplayString(SymbolDisplayFormat);
        }

        public static bool IsClusterType(
            this INamedTypeSymbol t
            )
        {
            if (t.BaseType == null)
            {
                return false;
            }

            if (t.BaseType!.ToDisplayString() != typeof(DefaultCluster).FullName)
            {
                return false;
            }

            return true;
        }

        public static void ScanForRequiredSyntaxes(
            this INamedTypeSymbol clusterType,
            out List<MethodDeclarationSyntax> bindMethodSyntaxes,
            out List<CompilationUnitSyntax> compilationUnitSyntaxes
            )
        {
            bindMethodSyntaxes = new List<MethodDeclarationSyntax>();
            compilationUnitSyntaxes = new List<CompilationUnitSyntax>();

            var bindMethods = (
                from member in clusterType.GetMembers()
                where member is IMethodSymbol
                let method = member as IMethodSymbol
                where method.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == typeof(DpdtBindingMethodAttribute).FullName)
                select method
                ).ToArray();

            if (bindMethods.Length == 0)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                    $"Something wrong with type {clusterType.ToDisplayString()} : no bind methods found. Please add at least one bind method or remove this class."
                    );
            }

            foreach (var bindMethod in bindMethods)
            {
                if (bindMethod.Parameters.Length != 0)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.BindMethodHasArguments,
                        $"Bind method {bindMethod.Name} of cluster {clusterType.Name} has parameters. It should be parameterless."
                        );
                }

                var bindMethodRefs = bindMethod.DeclaringSyntaxReferences;

                if (bindMethodRefs.Length != 1)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                        $"Something wrong with method {bindMethod.ToDisplayString()} : refs to bind method = {bindMethodRefs.Length}, should only one."
                        );
                }

                var bindMethodRef = bindMethodRefs[0];

                var bindMethodSyntax = (MethodDeclarationSyntax)bindMethodRef.GetSyntax();
                bindMethodSyntaxes.Add(bindMethodSyntax);

                var compilationUnitSyntax = bindMethodSyntax.Root<CompilationUnitSyntax>();
                if (compilationUnitSyntax is not null)
                {
                    //compilationUnitSyntax can repeat
                    if (compilationUnitSyntaxes.All(cus => cus.ToString() != compilationUnitSyntax.ToString()))
                    {
                        compilationUnitSyntaxes.Add(compilationUnitSyntax);
                    }
                }
            }
        }


        public static string ToSource(
            this Accessibility a
            )
        {
            switch (a)
            {
                case Accessibility.Private:
                    return "private";
                case Accessibility.ProtectedAndInternal:
                    return "protected internal";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "internal";
                case Accessibility.Public:
                    return "public";
                case Accessibility.ProtectedOrInternal:
                case Accessibility.NotApplicable:
                default:
                    throw new ArgumentOutOfRangeException(a.ToString());
            }

        }


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

        public static bool TryGetCompileTimeString(
            this ExpressionSyntax expression,
            SemanticModelDecorator semanticModel,
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
                result = constant.Value!.ToString()!;
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
            var roslynTypeFullName = target.ToDisplayString();

            if (StringComparer.InvariantCultureIgnoreCase.Compare(roslynTypeFullName, subjectTypeFullName) == 0)
            {
                return true;
            }

            foreach (INamedTypeSymbol @interface in target.AllInterfaces)
            {
                var roslynInterfaceFullName = @interface.ToDisplayString();

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
