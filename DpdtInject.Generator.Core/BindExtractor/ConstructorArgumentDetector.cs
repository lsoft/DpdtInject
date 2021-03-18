using DpdtInject.Generator.Core.Binding;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Core.BindExtractor
{
    public class ConstructorArgumentDetector
    {
        private readonly BindConstructorChooser _constructorChooser;

        public ConstructorArgumentDetector(
            BindConstructorChooser constructorChooser
            )
        {

            if (constructorChooser is null)
            {
                throw new ArgumentNullException(nameof(constructorChooser));
            }

            _constructorChooser = constructorChooser;
        }

        public int AppendUnknown(
            INamedTypeSymbol typeSymbol,
            ref List<DetectedConstructorArgument> constructorArguments
            )
        {
            if (typeSymbol is null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }

            var chosenConstructor = _constructorChooser.Choose(
                typeSymbol,
                constructorArguments
                );

            var appended = 0;
            foreach (var cParameter in chosenConstructor.Parameters)
            {
                var cParameterName = cParameter.Name;
                var cParameterType = cParameter.Type;

                var found = constructorArguments.FirstOrDefault(ca => ca.Name == cParameterName);
                if (found is null)
                {
                    constructorArguments.Add(
                        new DetectedConstructorArgument(
                            cParameterName,
                            cParameterType,
                            cParameter.RefKind,
                            cParameter.HasExplicitDefaultValue,
                            () => cParameter.ExplicitDefaultValue
                            )
                        );

                    appended++;
                }
            }

            return appended;
        }
    }
}
