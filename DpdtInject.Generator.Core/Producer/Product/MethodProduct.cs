using System;
using System.Collections.Generic;
using DpdtInject.Generator.Core.ArgumentWrapper;
using DpdtInject.Generator.Core.Binding;
using Microsoft.CodeAnalysis;
using DpdtInject.Injector.Src.Helper;
using DpdtInject.Injector.Src;

namespace DpdtInject.Generator.Core.Producer.Product
{
    public interface IMethodProduct : IWritable
    {
        string MethodName
        {
            get;
        }

        IMethodResult MethodResult
        {
            get;
        }

        string GetWrappedMethodName(DpdtArgumentWrapperTypeEnum wrapperType);
    }


    public class AdvancedMethodProduct : IMethodProduct
    {
        private readonly Func<string, string, string> _fullMethodBody;

        public string MethodName
        {
            get;
        }
        public IMethodResult MethodResult
        {
            get;
        }

        public AdvancedMethodProduct(
            string methodName,
            IMethodResult methodResult,
            Func<string, string, string> fullMethodBody //(methodName, resultName) => methodBody
            )
        {
            if (methodName is null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            if (methodResult is null)
            {
                throw new ArgumentNullException(nameof(methodResult));
            }

            if (fullMethodBody is null)
            {
                throw new ArgumentNullException(nameof(fullMethodBody));
            }

            MethodName = methodName;
            MethodResult = methodResult;
            _fullMethodBody = fullMethodBody;
        }

        public string GetWrappedMethodName(DpdtArgumentWrapperTypeEnum wrapperType)
        {
            return
                $"{MethodName}{wrapperType.GetPostfix()}";
        }

        public void Write(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            var returnName = MethodResult.GetString(sng);

            var methodBody = _fullMethodBody(MethodName, returnName);

            writer.WriteLine2(methodBody);
        }
    }

    public static class MethodProductFactory
    {
        public static IMethodProduct Create(
            IMethodSymbol methodSymbol,
            IReadOnlyList<DetectedMethodArgument> constructorArguments,
            Func<string, string, string> methodBody //(methodName, methodDeclaration) => methodBody
            )
        {
            var methodName = methodSymbol.Name;
            var methodResult = new TypeMethodResult(methodSymbol.ReturnType);

            var methodDeclaration = $@"{GetReturnModifiers(methodSymbol)} {methodSymbol.ReturnType.ToGlobalDisplayString()} {methodName}({constructorArguments.Join(ca => ca.GetDeclarationSyntax(), ",")})";

            return new AdvancedMethodProduct(
                methodName,
                methodResult,
                (methodName1, returnName1) => methodBody(methodName, methodDeclaration)
                );
        }

        public static IMethodProduct Create(
            string methodName,
            IMethodResult methodResult,
            Func<string, string, string> fullMethodBody //(methodName, returnType) => methodBody
            )
        {
            return new AdvancedMethodProduct(
                methodName,
                methodResult,
                fullMethodBody
                );
        }

        private static string GetReturnModifiers(IMethodSymbol methodSymbol)
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
    }

    public interface IMethodResult
    {
        string GetString(ShortTypeNameGenerator sng);
    }


    public class StringMethodResult: IMethodResult
    {
        private readonly string _type;

        public StringMethodResult(string type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _type = type;
        }

        public string GetString(ShortTypeNameGenerator sng)
        {
            return _type;
        }
    }

    public class TypeMethodResult : IMethodResult
    {
        private readonly ITypeSymbol _type;

        public TypeMethodResult(ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _type = type;
        }

        public string GetString(ShortTypeNameGenerator sng)
        {
            return sng.GetShortName(_type);
        }
    }
}
