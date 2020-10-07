using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Helpers
{
    public static class CompilationHelper
    {
        public static INamedTypeSymbol Func(
            this Compilation compilation,
            params ITypeSymbol[] genericParameters
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            return
                compilation.GetTypeByMetadataName("System.Func`" + genericParameters.Length)!
                .Construct(genericParameters)
                ;
        }

        public static INamedTypeSymbol SystemType(
            this Compilation compilation
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            return compilation.GetTypeByMetadataName("System.Type")!;
        }

        public static INamedTypeSymbol Object(
            this Compilation compilation
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            return compilation.GetTypeByMetadataName("System.Object")!;
        }

        public static INamedTypeSymbol Bool(
            this Compilation compilation
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            return compilation.GetTypeByMetadataName("System.Boolean")!;
        }

        public static INamedTypeSymbol Void(
            this Compilation compilation
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            return compilation.GetTypeByMetadataName("System.Void")!;
        }
    }
}
