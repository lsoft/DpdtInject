using DpdtInject.Generator.Core.Binding;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Generator.Core.Producer;
using DpdtInject.Generator.Core.Binding.Settings.Constructor;

namespace DpdtInject.Generator.Core.BindExtractor
{
    public class BindConstructorChooser
    {
        /// <summary>
        /// <see cref="BindConstructorChooser"/> is stateless, so we can use it as a singleton.
        /// </summary>
        public static readonly BindConstructorChooser Instance = new BindConstructorChooser();

        private BindConstructorChooser()
        {
        }

        /// <summary>
        /// Choose the constructor base on constructor argument and constructor setting.
        /// Throws exception in no constructor matched.
        /// </summary>
        /// <param name="fullBindToTypeName">Type we choosing the constructor in</param>
        /// <param name="constructorSetting">Constructor settings (if exists)</param>
        /// <param name="constructorArguments">Binding clause constructor arguments</param>
        /// <returns>Choosed (best matched) constructor</returns>
        public IMethodSymbol Choose(
            INamedTypeSymbol fullBindToTypeName,
            ConstructorSetting? constructorSetting,
            IReadOnlyList<DetectedMethodArgument> constructorArguments
            )
        {
            if (fullBindToTypeName is null)
            {
                throw new ArgumentNullException(nameof(fullBindToTypeName));
            }

            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }

            //constructor argument names exists
            //we should choose appropriate constructor
            IMethodSymbol? chosenConstructor = null;
            foreach (var constructor in fullBindToTypeName.InstanceConstructors)
            {
                //if constructor setting is here, we need check current constructor to be matched against it
                if (constructorSetting is not null)
                {
                    if (!CheckConstructor(constructorSetting, constructor))
                    {
                        //if not matched -> skip this constructor
                        continue;
                    }
                }

                //check for named arguments exists
                if (!ContainsAllNamedArguments(constructor, constructorArguments))
                {
                    continue;
                }

                //all checks are green, so we need to decide - replace the constructor we found earlier or do not
                if (chosenConstructor == null)
                {
                    //no constructor has been choosen earlier, so save the current one
                    chosenConstructor = constructor;
                }
                else
                {
                    //we need to compare 2 constructors
                    //here is some kind of hardcoded heuristic: we prefer constructor with fewer parameters
                    if (chosenConstructor.Parameters.Length > constructor.Parameters.Length)
                    {
                        //the current constructor has fewer parameters, so take it
                        chosenConstructor = constructor;
                    }
                }
            }

            if (chosenConstructor == null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.ConstructorArgumentMiss,
                    $@"Type {fullBindToTypeName.Name} does not contains constructor with arguments ({string.Join(",", constructorArguments.Select(ca => ca.Name))})",
                    fullBindToTypeName.Name
                    );
            }

            return chosenConstructor;
        }

        #region constructor setting related code

        private bool CheckConstructor(
            ConstructorSetting constructorSetting, 
            IMethodSymbol constructor
            )
        {
            switch (constructorSetting.CheckMode)
            {
                case ConstructorSettingsEnum.AllAndOrder:
                    return CheckAllAndOrderConstructor(constructorSetting, constructor);
                case ConstructorSettingsEnum.SubsetAndOrder:
                    return CheckSubsetAndOrderConstructor(constructorSetting, constructor);
                case ConstructorSettingsEnum.SubsetNoOrder:
                    return CheckSubsetNoOrderConstructor(constructorSetting, constructor);
                default:
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.ConstructorArgumentMiss,
                        $@"Unknown constructor settings for {constructor.ContainingType?.ToDisplayString() ?? "<no type>"})"
                        );
            }
        }

        private bool CheckAllAndOrderConstructor(
            ConstructorSetting constructorSetting,
            IMethodSymbol constructor
            )
        {
            if (constructorSetting.ConstructorArgumentsTypes.Count != constructor.Parameters.Length)
            {
                return false;
            }

            for (var i = 0; i < constructorSetting.ConstructorArgumentsTypes.Count; i++)
            {
                var need = constructorSetting.ConstructorArgumentsTypes[i];
                var exists = constructor.Parameters[i].Type;

                if(!SymbolEqualityComparer.IncludeNullability.Equals(exists, need))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckSubsetAndOrderConstructor(
            ConstructorSetting constructorSetting,
            IMethodSymbol constructor
            )
        {
            if (constructorSetting.ConstructorArgumentsTypes.Count > constructor.Parameters.Length)
            {
                return false;
            }

            var startIndex = 0;
            var ps = constructor.Parameters.ToList();
            foreach (var cat in constructorSetting.ConstructorArgumentsTypes)
            {
                startIndex = ps.FindIndex(startIndex, p => SymbolEqualityComparer.IncludeNullability.Equals(cat, p.Type));
                if (startIndex < 0)
                {
                    return false;
                }

                startIndex++;
            }

            return true;
        }

        private bool CheckSubsetNoOrderConstructor(
            ConstructorSetting constructorSetting,
            IMethodSymbol constructor
            )
        {
            if (constructorSetting.ConstructorArgumentsTypes.Count > constructor.Parameters.Length)
            {
                return false;
            }

           var groups = (
                from cat in constructorSetting.ConstructorArgumentsTypes
                let catn = cat.ToDisplayString()
                group cat by catn into catg
                select catg).ToList();

            foreach (var group in groups)
            {
                var needType = group.First();
                var needCount = group.Count();

                var existsCount = constructor.Parameters
                    .Where(p => SymbolEqualityComparer.IncludeNullability.Equals(p.Type, needType))
                    .Count()
                    ;

                if (existsCount < needCount)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Check if the constructor has all named argument we need to have in.
        /// </summary>
        /// <param name="constructor">Choosed constructor.</param>
        /// <param name="constructorArguments">Arguments to check.</param>
        /// <returns>true - if constructor has contain all needed arguments.</returns>
        private bool ContainsAllNamedArguments(
            IMethodSymbol constructor,
            IReadOnlyList<DetectedMethodArgument> constructorArguments
            )
        {
            if (constructorArguments.Count == 0)
            {
                return true;
            }

            foreach (var ca in constructorArguments)
            {
                var caName = ca.Name;

                var cp = constructor.Parameters.FirstOrDefault(j => j.Name == caName);
                if (cp is null)
                {
                    return false;
                }

                if (!(ca.Type is null))
                {
                    if (cp.Type.ToGlobalDisplayString() != ca.Type!.ToGlobalDisplayString())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
