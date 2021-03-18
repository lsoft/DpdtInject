using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;


namespace DpdtInject.Generator.Core.Producer
{
    public static class NameHelper
    {
        public static string GetSpecialName(
            this ITypeSymbol wrapperSymbol
            )
        {
            return wrapperSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).EscapeSpecialTypeSymbols();
        }
    }
}