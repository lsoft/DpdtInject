using System;
using System.Collections.Generic;
using DpdtInject.Generator.Core.Helpers;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Producer.Product
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
            itwMethods.WriteLine($"#region {BindFrom.ToFullDisplayString()}");
            itwMethods.WriteLine();

            foreach (var (resolutionProduct, isLast1) in ResolutionProducts.IterateWithLastSignal())
            {
                resolutionProduct.WriteMethods(itwMethods, sng);

                resolutionProduct.WriteInterface(itwInterfaces, sng);
                if (!isLast0 || !isLast1)
                {
                    itwInterfaces.WriteLine(",");
                }

                resolutionProduct.NonGenericGetTuple.Write(itwNonGenericInterfaces, sng);
                if (!isLast0 || !isLast1)
                {
                    itwNonGenericInterfaces.WriteLine(",");
                }

                resolutionProduct.NonGenericGetAllTuple.Write(itwNonGenericGetAllInterfaces, sng);
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