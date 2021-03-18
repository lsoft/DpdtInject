using System;
using System.Collections.Generic;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Producer.Product;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Producer.ClassProducer.Product
{
    internal class FactoryClassProduct : IProducedClassProduct
    {
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
        public IReadOnlyList<DetectedConstructorArgument> Unknowns
        {
            get;
        }

        public FactoryClassProduct(
            ITypeSymbol bindFromType,
            ITypeSymbol factoryType,
            IReadOnlyList<IMethodProduct> methodProducts,
            IReadOnlyList<DetectedConstructorArgument> unknowns
            )
        {
            if (bindFromType is null)
            {
                throw new ArgumentNullException(nameof(bindFromType));
            }

            if (factoryType is null)
            {
                throw new ArgumentNullException(nameof(factoryType));
            }

            if (methodProducts is null)
            {
                throw new ArgumentNullException(nameof(methodProducts));
            }

            if (unknowns is null)
            {
                throw new ArgumentNullException(nameof(unknowns));
            }

            BindFromType = bindFromType;
            BindToType = factoryType;
            MethodProducts = methodProducts;
            Unknowns = unknowns;
        }

        public string GetSourceCode()
        {
            var writer = new IndentedTextWriter2(2, false);
            var sng = new ShortTypeNameGenerator();

            foreach (var methodProduct in MethodProducts)
            {
                methodProduct.Write(writer, sng);
            }

            return ($@"
{sng.GetCombinedUsings()}

namespace {BindToType.ContainingNamespace.ToDisplayString()}
{{
    public partial class {BindToType.Name} : {BindFromType.ToDisplayString()}
    {{
        {Unknowns.Join(u => $"private readonly {u.Type!.ToDisplayString()} {u.Name};")}

        public {BindToType.Name}(
            {Unknowns.Join(u => $"{u.Type!.ToDisplayString()} {u.Name}", ",")}
            )
        {{
            {Unknowns.Join(u => $"this.{u.Name} = {u.Name};")}
        }}

        {writer.GetResultString()}
    }}
}}
");
        }
    }
}
