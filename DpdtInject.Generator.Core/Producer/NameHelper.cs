using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using System;

namespace DpdtInject.Generator.Core.Producer
{
    public static class NameHelper
    {
        private static readonly SymbolDisplayFormat SymbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

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

            return s.ToDisplayString(SymbolDisplayFormat);
        }


        public static string GetSpecialName(
            this ITypeSymbol wrapperSymbol
            )
        {
            return wrapperSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).EscapeSpecialTypeSymbols();
        }
    }
}