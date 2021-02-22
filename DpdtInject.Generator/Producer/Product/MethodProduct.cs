using System;
using System.Collections.Generic;
using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Binding;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.Product
{
    public class MethodProduct
    {
        public IReadOnlyList<DetectedConstructorArgument> ConstructorArguments
        {
            get;
        }

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
            IMethodSymbol methodSymbol,
            IReadOnlyList<DetectedConstructorArgument> constructorArguments,
            Func<IMethodSymbol, string, string> methodBody
            )
        {
            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }

            if (methodBody is null)
            {
                throw new ArgumentNullException(nameof(methodBody));
            }

            MethodName = methodSymbol.Name;
            ReturnType = methodSymbol.ReturnType;
            ConstructorArguments = constructorArguments;
            MethodBody = methodBody(
                methodSymbol, 
                GetMethodDeclaration(methodSymbol, DpdtArgumentWrapperTypeEnum.None)
                );
        }

        public MethodProduct(
            string methodName,
            ITypeSymbol returnType,
            Func<string, ITypeSymbol, string> fullMethodBody
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

            if (fullMethodBody is null)
            {
                throw new ArgumentNullException(nameof(fullMethodBody));
            }

            MethodName = methodName;
            ReturnType = returnType;
            ConstructorArguments = new List<DetectedConstructorArgument>();
            MethodBody = fullMethodBody(methodName, returnType);
        }

        public string GetMethodDeclaration(
            IMethodSymbol methodSymbol,
            DpdtArgumentWrapperTypeEnum wrapperType
            )
        {
            var methodName = GetMethodName(wrapperType);

            return $@"
{GetReturnModifiers(methodSymbol)} {ReturnType.ToDisplayString()} {methodName}({ConstructorArguments.Join(ca => ca.GetDeclarationSyntax(), ",")})
";
        }

        private string GetReturnModifiers(IMethodSymbol methodSymbol)
        {
            if (methodSymbol.ReturnsByRefReadonly)
            {
                return "ref readonly";
            }
            if (methodSymbol.ReturnsByRef)
            {
                return "ref";
            }

            return string.Empty;
        }

        public string GetMethodName(DpdtArgumentWrapperTypeEnum wrapperType)
        {
            return
                $"{MethodName}{wrapperType.GetPostfix()}";
        }
    }
}
