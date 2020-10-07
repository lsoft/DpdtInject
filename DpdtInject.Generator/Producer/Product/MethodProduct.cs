using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Injector;
using Microsoft.CodeAnalysis;
using System;

namespace DpdtInject.Generator.Producer
{
    public class MethodProduct
    {
        public string MethodName
        {
            get;
        }
        public ITypeSymbol ReturnType
        {
            get;
        }
        public string MethodBody
        {
            get;
        }

        public MethodProduct(
            string methodName,
            ITypeSymbol returnType,
            Func<string, ITypeSymbol, string> methodBody
            )
        {
            if (methodName is null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            if (returnType is null)
            {
                throw new ArgumentNullException(nameof(returnType));
            }

            if (methodBody is null)
            {
                throw new ArgumentNullException(nameof(methodBody));
            }

            MethodName = methodName;
            ReturnType = returnType;
            MethodBody = methodBody(methodName, returnType);
        }

        public string GetMethodName(DpdtArgumentWrapperTypeEnum wrapperType)
        {
            return
                $"{MethodName}{wrapperType.GetPostfix()}";
        }
    }
}
