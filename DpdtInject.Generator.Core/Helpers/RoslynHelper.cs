using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Generator.Core.Producer;

namespace DpdtInject.Generator.Core.Helpers
{
    public static class RoslynHelper
    {
        public static string GetJoinedParametersNameAndType(
            this IPropertySymbol property
            )
        {
            var parameters = string.Empty;
            if (property.Parameters.Length > 0)
            {
                parameters = string.Join(
                    ",",
                    property.Parameters.Select(p => p.Type.ToGlobalDisplayString() + " " + p.Name)
                    );
            }

            return parameters;
        }

        public static string GetJoinedParametersName(
            this IPropertySymbol property
            )
        {
            var parameters = string.Empty;
            if (property.Parameters.Length > 0)
            {
                parameters = string.Join(
                    ",",
                    property.Parameters.Select(p => p.Name)
                    );
            }

            return parameters;
        }


        public static bool IsClusterType(
            this INamedTypeSymbol t
            )
        {
            if (t.BaseType == null)
            {
                return false;
            }

            if (t.BaseType!.ToFullDisplayString() != typeof(DefaultCluster).FullName)
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
                where method.GetAttributes().Any(a => a.AttributeClass?.ToGlobalDisplayString() == typeof(DpdtBindingMethodAttribute).ToGlobalDisplayString())
                select method
                ).ToArray();

            if (bindMethods.Length == 0)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectBinding_IncorrectConfiguration,
                    $"Something wrong with type {clusterType.ToGlobalDisplayString()} : no bind methods found. Please add at least one bind method or remove this class."
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
                        $"Something wrong with method {bindMethod.ToGlobalDisplayString()} : refs to bind method = {bindMethodRefs.Length}, should only one."
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

        public static IEnumerable<INamedTypeSymbol> GetAllTypes(
            this INamespaceSymbol @namespace,
            Func<INamedTypeSymbol, bool> predicate
            )
        {
            if (@namespace == null)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var type in @namespace.GetTypeMembers())
            {
                foreach (var nestedType in type.GetNestedTypes(predicate))
                {
                    yield return nestedType;
                }
            }

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            {
                foreach (var type in nestedNamespace.GetAllTypes(predicate))
                {
                    yield return type;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetNestedTypes(
            this INamedTypeSymbol type,
            Func<INamedTypeSymbol, bool> predicate
            )
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (predicate(type))
            {
                yield return type;
            }

            foreach (var nestedType in type.GetTypeMembers().SelectMany(nestedType => nestedType.GetNestedTypes(predicate)))
            {
                yield return nestedType;
            }
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
            this ITypeSymbol source,
            ITypeSymbol target
            )
        {
            if (SymbolEqualityComparer.Default.Equals(source, target))
            {
                return true;
            }
            if (source is INamedTypeSymbol ntSource)
            {
                if (SymbolEqualityComparer.Default.Equals(ntSource.ConstructedFrom, target))
                {
                    return true;
                }
            }

            foreach (INamedTypeSymbol @interface in source.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(@interface, target))
                {
                    return true;
                }
            }

            if (source.BaseType != null && !SymbolEqualityComparer.Default.Equals(source.BaseType, source))
            {
                if (CanBeCastedTo(source.BaseType, target))
                {
                    return true;
                }
            }

            foreach (INamedTypeSymbol @interface in source.AllInterfaces)
            {
                var r = CanBeCastedTo(
                    @interface,
                    target
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
