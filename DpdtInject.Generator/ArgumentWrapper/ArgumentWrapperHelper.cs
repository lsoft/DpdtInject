using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Generator.ArgumentWrapper
{
    public static class ArgumentWrapperHelper
    {
        public static IEnumerable<(DpdtArgumentWrapperTypeEnum, ITypeSymbol)> GenerateWrapperTypes(
            this ITypeSymbol type,
            Compilation compilation
            )
        {
            foreach (DpdtArgumentWrapperTypeEnum wrapperType in Enum.GetValues(typeof(DpdtArgumentWrapperTypeEnum)))
            {
                INamedTypeSymbol wrapperSymbol;
                switch (wrapperType)
                {
                    case DpdtArgumentWrapperTypeEnum.None:
                        continue;
                    case DpdtArgumentWrapperTypeEnum.Func:
                        wrapperSymbol = compilation.GetTypeByMetadataName("System.Func`1")!;
                        wrapperSymbol = wrapperSymbol.Construct(type);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(wrapperType.ToString());
                }

                yield return (wrapperType, wrapperSymbol);
            }
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
