using DpdtInject.Generator.Core.Binding;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Generator.Core.Producer;
using System.Text.RegularExpressions;
using DpdtInject.Generator.Core.Binding.Settings.Constructor;

namespace DpdtInject.Generator.Core.BindExtractor
{
    public class BindConstructorChooser
    {
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
                if (constructorSetting is not null)
                {
                    if (!CheckConstructor(constructorSetting, constructor))
                    {
                        continue;
                    }
                }

                if (!ContainsAllNamedArguments(constructor, constructorArguments))
                {
                    continue;
                }

                if (chosenConstructor == null)
                {
                    chosenConstructor = constructor;
                }
                else
                {
                    if (chosenConstructor.Parameters.Length > constructor.Parameters.Length)
                    {
                        //here is some kind of hardcoded heuristic: we prefer constructor with fewer parameters
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
