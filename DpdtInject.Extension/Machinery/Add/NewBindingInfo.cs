using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Extension.Machinery.Add
{
    public class NewBindingInfo
    {
        public IReadOnlyCollection<INamedTypeSymbol> BindFroms
        {
            get;
        }
        public INamedTypeSymbol BindTo
        {
            get;
        }
        public IMethodSymbol Constructor
        {
            get;
        }
        public List<IParameterSymbol> ManualConstructorArguments
        {
            get;
        }
        public BindScopeEnum BindScope
        {
            get;
        }
        public bool IsConditional
        {
            get;
        }

        public bool IsBindingComplete
        {
            get
            {
                if (ManualConstructorArguments.Count > 0)
                {
                    return false;
                }
                if (IsConditional)
                {
                    return false;
                }
                if (BindScope == BindScopeEnum.Constant)
                {
                    return false;
                }
                if (BindTo.IsGenericType)
                {
                    return false;
                }

                return true;
            }
        }

        public NewBindingInfo(
            IReadOnlyCollection<INamedTypeSymbol> bindFroms,
            INamedTypeSymbol bindTo,
            IMethodSymbol constructor,
            List<IParameterSymbol> manualConstructorArguments,
            BindScopeEnum bindScope,
            bool isConditional
            )
        {
            if (bindFroms is null)
            {
                throw new ArgumentNullException(nameof(bindFroms));
            }

            if (bindTo is null)
            {
                throw new ArgumentNullException(nameof(bindTo));
            }

            if (constructor is null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            if (manualConstructorArguments is null)
            {
                throw new ArgumentNullException(nameof(manualConstructorArguments));
            }

            BindFroms = bindFroms;
            BindTo = bindTo;
            Constructor = constructor;
            ManualConstructorArguments = manualConstructorArguments;
            BindScope = bindScope;
            IsConditional = isConditional;
        }

        public IReadOnlyList<UsingDirectiveSyntax> GetNewUsings(
            )
        {
            var result = new Dictionary<string, UsingDirectiveSyntax>();

            foreach (var bindFrom in BindFroms)
            {
                var key = bindFrom.ContainingNamespace.ToDisplayString();
                result[key] = 
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(
                            " " + key
                            )
                        ).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    ;
            }
            {
                var key = BindTo.ContainingNamespace.ToDisplayString();
                result[key] = 
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(
                            " " + key
                            )
                        ).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    ;
            }
            foreach (var constructorArgument in ManualConstructorArguments)
            {
                var key = constructorArgument.Type.ContainingNamespace.ToDisplayString();
                result[key] = 
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(
                            " " + key
                            )
                        ).WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    ;
            }

            var constructorArgumentKey = typeof(ConstructorArgument).Namespace;
            result[constructorArgumentKey] =
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.ParseName(
                        " " + constructorArgumentKey
                        )
                    );

            return result.Values.ToList();
        }

        public IEnumerable<UsingDirectiveSyntax> GetUniqueUsings(
            IReadOnlyList<UsingDirectiveSyntax> existingNamespaces
            )
        {
            var newAll = GetNewUsings();
            foreach (var newn in newAll)
            {
                if (existingNamespaces.Any(en => en.Name.ToString() == newn.Name.ToString()))
                {
                    continue;
                }

                yield return newn;
            }
        }

    }
}
