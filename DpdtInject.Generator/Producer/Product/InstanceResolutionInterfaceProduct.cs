using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.Product
{
    public class InstanceResolutionInterfaceProduct
    {
        public ITypeSymbol BindFrom
        {
            get;
        }
        public IReadOnlyList<ResolutionProduct> ResolutionProducts
        {
            get;
        }

        public InstanceResolutionInterfaceProduct(
            ITypeSymbol bindFrom,
            List<ResolutionProduct> resolutionProducts
            )
        {
            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            if (resolutionProducts is null)
            {
                throw new ArgumentNullException(nameof(resolutionProducts));
            }
            BindFrom = bindFrom;
            ResolutionProducts = resolutionProducts;
        }

        internal void Write(
            bool isLast0,
            IndentedTextWriter2 itwMethods,
            IndentedTextWriter2 itwInterfaces,
            IndentedTextWriter2 itwNonGenericInterfaces,
            IndentedTextWriter2 itwNonGenericGetAllInterfaces,
            ShortTypeNameGenerator sng
            )
        {
            itwMethods.WriteLine($"#region {BindFrom.ToDisplayString()}");
            itwMethods.WriteLine();

            foreach (var (resolutionProduct, isLast1) in ResolutionProducts.IterateWithLastSignal())
            {
                resolutionProduct.WriteMethods(itwMethods, sng);

                resolutionProduct.WriteInterface(itwInterfaces, sng);
                if (!isLast0 || !isLast1)
                {
                    itwInterfaces.WriteLine(",");
                }

                resolutionProduct.NonGenericGetTuple.WriteProduct(itwNonGenericInterfaces, sng);
                if (!isLast0 || !isLast1)
                {
                    itwNonGenericInterfaces.WriteLine(",");
                }

                resolutionProduct.NonGenericGetAllTuple.WriteProduct(itwNonGenericGetAllInterfaces, sng);
                if (!isLast0 || !isLast1)
                {
                    itwNonGenericGetAllInterfaces.WriteLine(",");
                }
            }

            itwMethods.WriteLine();
            itwMethods.WriteLine($"#endregion");

        }
    }

}