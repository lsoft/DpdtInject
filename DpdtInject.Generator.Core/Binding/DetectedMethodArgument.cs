using DpdtInject.Generator.Core.ArgumentWrapper;
using DpdtInject.Generator.Core.Producer;
using Microsoft.CodeAnalysis;
using System;
using DpdtInject.Injector.Src.Excp;
using System.Globalization;

namespace DpdtInject.Generator.Core.Binding
{
    public class DetectedMethodArgument
    {
        public int ArgumentIndex
        {
            get;
            private set;
        }

        public string Name
        {
            get;
        }

        public ITypeSymbol? Type
        {
            get;
            private set;
        }

        public RefKind RefKind
        {
            get;
        }

        public string? Body
        {
            get;
        }

        public bool HasExplicitDefaultValue
        {
            get;
        }
        public object? ExplicitDefaultValue
        {
            get;
        }

        public bool DefineInBindNode => !string.IsNullOrEmpty(Body);

        public DetectedMethodArgument(
            int argumentIndex,
            string name,
            string body
            )
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            ArgumentIndex = argumentIndex;
            Name = name;
            Type = null;
            RefKind = RefKind.None;
            Body = body;
            HasExplicitDefaultValue = false;
        }

        public DetectedMethodArgument(
            int argumentIndex,
            string name,
            ITypeSymbol type,
            RefKind refKind,
            bool hasExplicitDefaultValue,
            Func<object?> explicitDefaultValueFunc
            )
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (explicitDefaultValueFunc is null)
            {
                throw new ArgumentNullException(nameof(explicitDefaultValueFunc));
            }
            ArgumentIndex = argumentIndex;
            Name = name;
            Type = type;
            RefKind = refKind;
            Body = null;
            HasExplicitDefaultValue = hasExplicitDefaultValue;
            ExplicitDefaultValue = hasExplicitDefaultValue ? explicitDefaultValueFunc() : null;
        }

        internal void UpdateIndex(int argumentIndex)
        {
            ArgumentIndex = argumentIndex;
        }

        internal void UpdateType(ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type = type;
        }

        public ITypeSymbol GetUnwrappedType(
            )
        {
            if (Type is null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"constructorArgument.Type is null somehow"
                    );
            }

            if (Type.TryDetectWrapperType(
                out var wrapperType,
                out var internalType
                ))
            {
                return internalType;
            }

            return Type;
        }

        public string GetUsageSyntax()
        {
            if (Type is null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"constructorArgument.Type is null somehow"
                    );
            }

            return $"{GetUsageModifiers()} {Name}";
        }

        public string GetDeclarationSyntax()
        {
            if (Type is null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"constructorArgument.Type is null somehow"
                    );
            }

            if (HasExplicitDefaultValue)
            {
                return $"{GetDeclarationModifiers()} {Type.ToGlobalDisplayString()} {Name} = {GetExplicitValueCodeRepresentation()}";
            }
            else
            {
                return $"{GetDeclarationModifiers()} {Type.ToGlobalDisplayString()} {Name}";
            }
        }

        public string GetExplicitValueCodeRepresentation()
        {
            if (ExplicitDefaultValue is null)
            {
                return "default";
            }

            if (ExplicitDefaultValue is string sedv)
            {
                return $"\"{sedv}\"";
            }

            if (ExplicitDefaultValue is long ledv)
            {
                return $"{ledv}L";
            }

            if (ExplicitDefaultValue is ulong uledv)
            {
                return $"{uledv}UL";
            }

            if (ExplicitDefaultValue is char cedv)
            {
                return $"'{cedv}'";
            }

            if (ExplicitDefaultValue is float fedv)
            {
                return fedv.ToString(CultureInfo.InvariantCulture) + "f";
            }

            if (ExplicitDefaultValue is double dedv)
            {
                return dedv.ToString(CultureInfo.InvariantCulture) + "d";
            }

            if (ExplicitDefaultValue is decimal dcedv)
            {
                return dcedv.ToString(CultureInfo.InvariantCulture) + "m";
            }

            if (Type?.TypeKind == TypeKind.Enum)
            {
                return $"({Type.ToGlobalDisplayString()}){ExplicitDefaultValue}" ?? $"default({Type.ToGlobalDisplayString()})";
            }

            return ExplicitDefaultValue.ToString() ?? "default";
        }

        private string GetUsageModifiers()
        {
            switch (RefKind)
            {
                case RefKind.None:
                case RefKind.In:
                    return string.Empty;

                case RefKind.Ref:
                    return "ref";

                case RefKind.Out:
                    return "out";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetDeclarationModifiers()
        {
            switch (RefKind)
            {
                case RefKind.None:
                    return string.Empty;

                case RefKind.Ref:
                    return "ref";

                case RefKind.Out:
                    return "out";

                case RefKind.In:
                    return "in";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
