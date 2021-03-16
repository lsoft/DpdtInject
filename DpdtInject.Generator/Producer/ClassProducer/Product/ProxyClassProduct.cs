using System;
using System.Collections.Generic;
using DpdtInject.Generator.Producer.Product;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.ClassProducer.Product
{
    public class ProxyClassProduct : IProducedClassProduct
    {
        private readonly ITypeSymbol _sessionSaverType;

        public ITypeSymbol BindFromType
        {
            get;
        }
        public ITypeSymbol BindToType
        {
            get;
        }
        public IReadOnlyList<IMethodProduct> MethodProducts
        {
            get;
        }

        public ProxyClassProduct(
            ITypeSymbol bindFromType,
            ITypeSymbol proxyClassType,
            ITypeSymbol sessionSaverType,
            IReadOnlyList<IMethodProduct> methodProducts
            )
        {
            if (bindFromType is null)
            {
                throw new ArgumentNullException(nameof(bindFromType));
            }

            if (proxyClassType is null)
            {
                throw new ArgumentNullException(nameof(proxyClassType));
            }

            if (sessionSaverType is null)
            {
                throw new ArgumentNullException(nameof(sessionSaverType));
            }

            if (methodProducts is null)
            {
                throw new ArgumentNullException(nameof(methodProducts));
            }

            BindFromType = bindFromType;
            BindToType = proxyClassType;
            _sessionSaverType = sessionSaverType;
            MethodProducts = methodProducts;
        }

        public string GetSourceCode()
        {
            var writer = new IndentedTextWriter2(2, false);
            var sng = new ShortTypeNameGenerator();

            foreach (var methodProduct in MethodProducts)
            {
                methodProduct.Write(writer, sng);
            }

            var compilationUnit = DpdtInject.Generator.Properties.Resource.CarcassProxy;

            var fixedCompilationUnit = compilationUnit
                .ReplaceLineStartsWith(
                    "namespace",
                    $"namespace {BindToType.ContainingNamespace.ToDisplayString()}"
                    )
                .ReplaceLineStartsWith(
                    "using BindFromType",
                    $""
                    )
                .ReplaceLineStartsWith(
                    "using SessionSaver",
                    $""
                    )
                .CheckAndReplace(
                    "//PROXYPRODUCER: additonal usings",
                    sng.GetCombinedUsings()
                    )
                .CheckAndReplace(
                    nameof(CarcassProxy),
                    BindToType.Name
                    )
                .CheckAndReplace(
                    "BindFromType",
                    BindFromType.ToDisplayString()
                    )
                .CheckAndReplace(
                    "SessionSaver",
                    _sessionSaverType.ToDisplayString()
                    )
                .CheckAndReplace(
                    "//PROXYPRODUCER: put methods here",
                    writer.GetResultString()
                    )
                ;

            return fixedCompilationUnit;
        }
    }

}
