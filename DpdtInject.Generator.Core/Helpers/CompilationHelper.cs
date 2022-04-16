using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;
using System;

namespace DpdtInject.Generator.Core.Helpers
{
    public static class CompilationHelper
    {
        public static INamedTypeSymbol Func(
            this ITypeInfoProvider typeInfoProvider,
            params ITypeSymbol[] genericParameters
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            return
                typeInfoProvider.GetTypeByMetadataName("System.Func`" + genericParameters.Length)!
                    .Construct(genericParameters)
                    ;
        }

        public static INamedTypeSymbol ValueTask(
            this ITypeInfoProvider typeInfoProvider
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            return typeInfoProvider.GetTypeByMetadataName("System.Threading.Tasks.ValueTask")!;
        }

        public static INamedTypeSymbol ValueTask(
            this ITypeInfoProvider typeInfoProvider,
            params ITypeSymbol[] genericParameters
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            return
                typeInfoProvider.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`" + genericParameters.Length)!
                    .Construct(genericParameters)
                    ;
        }

        public static INamedTypeSymbol Exception(
            this ITypeInfoProvider typeInfoProvider
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            return typeInfoProvider.GetTypeByMetadataName("System.Exception")!;
        }

        public static INamedTypeSymbol SystemType(
            this ITypeInfoProvider typeInfoProvider
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            return typeInfoProvider.GetTypeByMetadataName("System.Type")!;
        }

        public static INamedTypeSymbol Object(
            this ITypeInfoProvider typeInfoProvider
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            return typeInfoProvider.GetTypeByMetadataName("System.Object")!;
        }

        public static INamedTypeSymbol Bool(
            this ITypeInfoProvider typeInfoProvider
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            return typeInfoProvider.GetTypeByMetadataName("System.Boolean")!;
        }

        public static INamedTypeSymbol Void(
            this ITypeInfoProvider typeInfoProvider
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            return typeInfoProvider.GetTypeByMetadataName("System.Void")!;
        }
    }
}
