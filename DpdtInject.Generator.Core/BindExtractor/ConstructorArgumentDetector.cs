using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Binding.Settings.Constructor;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector.Src.Excp;
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

        /// <summary>
        /// Choose constructor and append unknown arguments.
        /// Unknown argument is an argument that came from the constructor symbol,
        /// but not from the binding clause.
        /// </summary>
        /// <param name="typeSymbol">Type we choosing the constructor in</param>
        /// <param name="constructorSetting">Constructor settings (if exists)</param>
        /// <param name="constructorArguments">Binding clause constructor arguments</param>
        /// <returns>How many unknown arguments has been appended.</returns>
        public int ChooseConstructorAndAppendUnknownArguments(
            INamedTypeSymbol typeSymbol,
            ConstructorSetting? constructorSetting,
            ref List<DetectedMethodArgument> constructorArguments
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

            //choose the constructor base on constructor argument and constructor setting
            var chosenConstructor = _constructorChooser.Choose(
                typeSymbol,
                constructorSetting,
                constructorArguments
                );

            //update indexes for predefined constructor arguments
            foreach (var constructorArgument in constructorArguments)
            {
                var p = chosenConstructor.Parameters.FirstOrDefault(p => p.Name == constructorArgument.Name);

                if (p is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.ConstructorArgumentMiss,
                        $"Choosen constructor for [{typeSymbol.ToGlobalDisplayString()}] does not contains an argument with name {constructorArgument.Name}",
                        typeSymbol.ToGlobalDisplayString()
                        );
                    ;
                }

                var pindex = chosenConstructor.Parameters.IndexOf(p);

                constructorArgument.UpdateIndex(pindex);
                constructorArgument.UpdateType(p.Type);
            }

            //append unknown constructor arguments to the result collection
            var appended = 0;
            for(var i = 0; i < chosenConstructor.Parameters.Length; i++)
            {
                var cParameter = chosenConstructor.Parameters[i];

                var cParameterName = cParameter.Name;
                var cParameterType = cParameter.Type;

                var found = constructorArguments.FirstOrDefault(ca => ca.Name == cParameterName);
                
                if (found is not null)
                {
                    continue;
                }

                constructorArguments.Add(
                    new DetectedMethodArgument(
                        i,
                        cParameterName,
                        cParameterType,
                        cParameter.RefKind,
                        cParameter.HasExplicitDefaultValue,
                        () => cParameter.ExplicitDefaultValue
                        )
                    );

                appended++;
            }

            return appended;
        }

    }
}
