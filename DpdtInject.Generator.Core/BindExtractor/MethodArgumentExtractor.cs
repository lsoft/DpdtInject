using DpdtInject.Generator.Core.Binding;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Core.BindExtractor
{
    public class MethodArgumentExtractor
    {
        public List<DetectedMethodArgument> GetMethodArguments(
            IMethodSymbol methodSymbol
            )
        {
            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            return
                methodSymbol.Parameters.ConvertAll(
                    p => new DetectedMethodArgument(
                        p.Name, 
                        p.Type, 
                        p.RefKind,
                        p.HasExplicitDefaultValue,
                        () => p.ExplicitDefaultValue
                        )
                    );
        }
    }
}
