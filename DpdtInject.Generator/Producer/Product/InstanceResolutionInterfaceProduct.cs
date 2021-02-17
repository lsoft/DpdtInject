using System;
using System.Collections.Generic;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;

namespace DpdtInject.Generator.Producer.Product
{
    public class InstanceResolutionInterfaceProduct
    {
        public IReadOnlyList<ResolutionProduct> ResolutionProducts
        {
            get;
        }

        public InstanceResolutionInterfaceProduct(
            List<ResolutionProduct> resolutionProducts
            )
        {
            if (resolutionProducts is null)
            {
                throw new ArgumentNullException(nameof(resolutionProducts));
            }

            ResolutionProducts = resolutionProducts;
        }

        public string GetInterfaces()
        {
            if (ResolutionProducts.Count == 0)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, "ResolutionProducts must be set");
            }
            
            return $@"
{ResolutionProducts.Join(r => r.GetInterface(), ",")}
";
        }

        public string GetMethods()
        {
            if(ResolutionProducts.Count == 0)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, "ResolutionProducts must be set");
            }

            return $@"
{ResolutionProducts.Join(r => r.GetMethods())}
";
        }
    }

}