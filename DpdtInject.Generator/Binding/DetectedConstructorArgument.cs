using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using System;

namespace DpdtInject.Generator.Binding
{
    public class DetectedConstructorArgument
    {
        public string Name
        {
            get;
        }

        public ITypeSymbol? Type
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

        public DetectedConstructorArgument(
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
            Body = body;
            HasExplicitDefaultValue = false;
        }

        public DetectedConstructorArgument(
            string name,
            ITypeSymbol type,
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
                return $"{Type.ToDisplayString()} {Name} = {ExplicitDefaultValue?.ToString() ?? "null"}";
            }
            else
            {
                return $"{Type.ToDisplayString()} {Name}";
            }
        }
    }
}
