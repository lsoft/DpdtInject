using System;
using System.Collections.Generic;
using DpdtInject.Generator.Core.Helpers;
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
        public IReadOnlyList<IWritable> MembersProduct
        {
            get;
        }

        public ProxyClassProduct(
            ITypeSymbol bindFromType,
            ITypeSymbol proxyClassType,
            ITypeSymbol sessionSaverType,
            IReadOnlyList<IWritable> membersProducts
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

            if (membersProducts is null)
            {
                throw new ArgumentNullException(nameof(membersProducts));
            }

            BindFromType = bindFromType;
            BindToType = proxyClassType;
            _sessionSaverType = sessionSaverType;
            MembersProduct = membersProducts;
        }

        public string GetSourceCode()
        {
            var writer = new IndentedTextWriter2(2, false);
            var sng = new ShortTypeNameGenerator();

            foreach (var methodProduct in MembersProduct)
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
