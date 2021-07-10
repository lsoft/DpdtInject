using DpdtInject.Generator.Core.Helpers;
using Microsoft.CodeAnalysis;
using System;
using DpdtInject.Injector.Src.Excp;
using System.Reflection;

namespace DpdtInject.Generator.Core.Producer
{
    public static class NameHelper
    {
        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        private static readonly Type _symbolDisplayCompilerInternalOptionsType = typeof(SymbolDisplayFormat).Assembly.GetType("Microsoft.CodeAnalysis.SymbolDisplayCompilerInternalOptions")!;
        private static readonly ConstructorInfo _constructor = typeof(SymbolDisplayFormat).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            CallingConventions.Any,
            new Type[]
            {
                _symbolDisplayCompilerInternalOptionsType,
                typeof(SymbolDisplayGlobalNamespaceStyle),
                typeof(SymbolDisplayTypeQualificationStyle),
                typeof(SymbolDisplayGenericsOptions),
                typeof(SymbolDisplayMemberOptions),
                typeof(SymbolDisplayParameterOptions),

                typeof(SymbolDisplayDelegateStyle),
                typeof(SymbolDisplayExtensionMethodStyle),
                typeof(SymbolDisplayPropertyStyle),
                typeof(SymbolDisplayLocalOptions),
                typeof(SymbolDisplayKindOptions),
                typeof(SymbolDisplayMiscellaneousOptions)
            },
            null
            )!;
        const int _symbolDisplayCompilerInternalOptions_UseArityForGenericTypes = 2;

        private static readonly SymbolDisplayFormat _reflectionFormat = (SymbolDisplayFormat)_constructor.Invoke(
            new object[]
            {
                        _symbolDisplayCompilerInternalOptions_UseArityForGenericTypes,
                        SymbolDisplayGlobalNamespaceStyle.Omitted, //default(SymbolDisplayGlobalNamespaceStyle),
                        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, //default(SymbolDisplayTypeQualificationStyle),
                        SymbolDisplayGenericsOptions.IncludeTypeParameters, //default(SymbolDisplayGenericsOptions),
                        default(SymbolDisplayMemberOptions),
                        default(SymbolDisplayDelegateStyle),

                        default(SymbolDisplayExtensionMethodStyle),
                        default(SymbolDisplayParameterOptions),
                        default(SymbolDisplayPropertyStyle),
                        default(SymbolDisplayLocalOptions),
                        default(SymbolDisplayKindOptions),
                        default(SymbolDisplayMiscellaneousOptions)
            });


        public static string ToGlobalDisplayString(
            this System.Type type
            )
        {
            var ds = type.FullName;

            if (ds == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    "No full name exists"
                    );
            }

            if (!ds.Contains("."))
            {
                return ds;
            }

            return "global::" + ds;
        }

        public static string ToGlobalDisplayString(
            this ISymbol symbol,
            SymbolDisplayFormat? format = null
            )
        {
            var ds = symbol.ToDisplayString(format ?? SymbolDisplayFormat.FullyQualifiedFormat);

            return ds;
        }

        public static string ToFullDisplayString(
            this ISymbol symbol,
            SymbolDisplayFormat? format = null
            )
        {
            var ds = symbol.ToDisplayString(format);

            return ds;
        }

        public static string ToFullyQualifiedName(
            this ISymbol s
            )
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.ToDisplayString(_symbolDisplayFormat);
        }

        public static string ToReflectionFormat(
            this INamedTypeSymbol s,
            bool deepLevel = false
            )
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            var reflectionTypeString = s.ToDisplayString(_reflectionFormat);
            if (s.TypeArguments.Length > 0)
            {
                reflectionTypeString += "[";
                foreach (var gta in s.TypeArguments)
                {
                    if (gta is not INamedTypeSymbol ngta)
                    {
                        throw new DpdtException(
                            DpdtExceptionTypeEnum.InternalError,
                            $"[{gta.ToDisplayString()}] is not a {nameof(INamedTypeSymbol)}"
                            );
                    }

                    reflectionTypeString += $"[{ngta.ToReflectionFormat(true)}]";
                    reflectionTypeString += ",";
                }
                reflectionTypeString = reflectionTypeString.Substring(0, reflectionTypeString.Length - 1) + "]";
            }

            if (deepLevel)
            {
                reflectionTypeString += ", " + s.ContainingAssembly.ToDisplayString();
            }

            return reflectionTypeString;
        }


        public static string GetSpecialName(
            this ITypeSymbol wrapperSymbol
            )
        {
            return wrapperSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).EscapeSpecialTypeSymbols();
        }
    }
}