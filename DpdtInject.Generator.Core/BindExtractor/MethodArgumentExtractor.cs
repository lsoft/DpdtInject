using DpdtInject.Generator.Core.Binding;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using DpdtInject.Injector.Src.Helper;

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

            var result = new List<DetectedMethodArgument>();

            for (var i = 0; i < methodSymbol.Parameters.Length; i++)
            {
                var p = methodSymbol.Parameters[i];

                result.Add(
                    new DetectedMethodArgument(
                        i,
                        p.Name,
                        p.Type,
                        p.RefKind,
                        p.HasExplicitDefaultValue,
                        () => p.ExplicitDefaultValue
                        )
                    );
            }

            return
                result;
        }
    }
}
