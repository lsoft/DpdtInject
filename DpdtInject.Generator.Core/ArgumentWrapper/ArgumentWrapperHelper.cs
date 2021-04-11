using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DpdtInject.Injector.Src;

namespace DpdtInject.Generator.Core.ArgumentWrapper
{
    public static class ArgumentWrapperHelper
    {
        public static int WrapperCountIncludeNone => Enum.GetValues(typeof(DpdtArgumentWrapperTypeEnum)).Length;

        public static IReadOnlyList<DpdtArgumentWrapperTypeEnum> GenerateWrapperEnumTypes(
            bool includeNone
            )
        {
            var result = new List<DpdtArgumentWrapperTypeEnum>();

            foreach (DpdtArgumentWrapperTypeEnum wrapperType in Enum.GetValues(typeof(DpdtArgumentWrapperTypeEnum)))
            {
                if (wrapperType == DpdtArgumentWrapperTypeEnum.None && !includeNone)
                {
                    continue;
                }

                result.Add(wrapperType);
            }

            return result;
        }

        public static ITypeSymbol GenerateWrapperTypes(
            this ITypeSymbol type,
            ITypeInfoProvider typeInfoProvider,
            DpdtArgumentWrapperTypeEnum wrapperType
            )
        {
            ITypeSymbol wrapperSymbol;
            switch (wrapperType)
            {
                case DpdtArgumentWrapperTypeEnum.None:
                    wrapperSymbol = type;
                    break;
                case DpdtArgumentWrapperTypeEnum.Func:
                    wrapperSymbol = typeInfoProvider.Func(type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(wrapperType.ToString());
            }

            return wrapperSymbol;
        }

        public static string GetPostfix(
            this DpdtArgumentWrapperTypeEnum wrapperType
            )
        {
            switch (wrapperType)
            {
                case DpdtArgumentWrapperTypeEnum.None:
                    return string.Empty;
                case DpdtArgumentWrapperTypeEnum.Func:
                    return "_Func";
                default:
                    throw new ArgumentOutOfRangeException(wrapperType.ToString());
            }
        }

        public static bool TryDetectWrapperType(
            this ITypeSymbol type,
            out DpdtArgumentWrapperTypeEnum wrapperType,
            [NotNullWhen(true)] out ITypeSymbol? internalType
            )
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var namedType = type as INamedTypeSymbol;
            if (namedType is null)
            {
                wrapperType = DpdtArgumentWrapperTypeEnum.None;
                internalType = null;
                return false;
            }

            var extractedName = type.Name;
            if (extractedName == "Func")
            {
                wrapperType = DpdtArgumentWrapperTypeEnum.Func;
                internalType = namedType.TypeArguments[0];
                return true;
            }

            wrapperType = DpdtArgumentWrapperTypeEnum.None;
            internalType = null;
            return false;
        }
    }
}
