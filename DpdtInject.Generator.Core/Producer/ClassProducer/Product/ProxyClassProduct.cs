﻿using System;
using System.Collections.Generic;
using DpdtInject.Generator.Core.Producer.Product;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Producer.ClassProducer.Product
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

            var compilationUnit = DpdtInject.Generator.Core.Properties.Resource.CarcassProxy;

            var fixedCompilationUnit = compilationUnit
                .ReplaceLineStartsWith(
                    "namespace",
                    $"namespace {BindToType.ContainingNamespace.ToFullDisplayString()}"
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
                    BindFromType.ToGlobalDisplayString()
                    )
                .CheckAndReplace(
                    "SessionSaver",
                    _sessionSaverType.ToGlobalDisplayString()
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
