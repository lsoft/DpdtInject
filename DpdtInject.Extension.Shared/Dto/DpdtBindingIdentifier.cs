using System;

namespace DpdtInject.Extension.Shared.Dto
{
    public class DpdtBindingIdentifier
    {
        public Guid BindingUniqueUnstableIdentifier
        {
            get;
        }

        public DpdtBindingIdentifier(Guid bindingUniqueUnstableIdentifier)
        {
            BindingUniqueUnstableIdentifier = bindingUniqueUnstableIdentifier;
        }
    }
}