using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Producer
{
    public class InstanceResolutionInterfaceProduct
    {
        public IReadOnlyList<ResolutionProduct> ResolutionProducts
        {
            get;
            private set;
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