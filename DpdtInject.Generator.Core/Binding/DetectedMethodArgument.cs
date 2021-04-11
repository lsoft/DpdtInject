using DpdtInject.Generator.Core.ArgumentWrapper;
using DpdtInject.Generator.Core.Producer;
using Microsoft.CodeAnalysis;
using System;
using DpdtInject.Injector.Src.Excp;

namespace DpdtInject.Generator.Core.Binding
{
    public class DetectedMethodArgument
    {
        public string Name
        {
            get;
        }

        public ITypeSymbol? Type
        {
            get;
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

            Name = name;
            Type = null;
            RefKind = RefKind.None;
            Body = body;
            HasExplicitDefaultValue = false;
        }

        public DetectedMethodArgument(
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

            Name = name;
            Type = type;
            RefKind = refKind;
            Body = null;
            HasExplicitDefaultValue = hasExplicitDefaultValue;
            ExplicitDefaultValue = hasExplicitDefaultValue ? explicitDefaultValueFunc() : null;
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
                return $"{GetDeclarationModifiers()} {Type.ToGlobalDisplayString()} {Name} = {ExplicitDefaultValue?.ToString() ?? "default"}";
            }
            else
            {
                return $"{GetDeclarationModifiers()} {Type.ToGlobalDisplayString()} {Name}";
            }
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
