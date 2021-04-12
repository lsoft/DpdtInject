using System;

namespace DpdtInject.Extension.Shared.Dto
{
    public interface IDpdtBindingTarget
    {
        Guid BindingUniqueUnstableIdentifier
        {
            get;
        }

        IDpdtClusterDetail ClusterDetail
        {
            get;
        }

        IDpdtBindingDetail BindingDetail
        {
            get;
        }
    }
}
