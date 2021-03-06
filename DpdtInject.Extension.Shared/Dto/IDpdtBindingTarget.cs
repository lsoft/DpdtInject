using System;

namespace DpdtInject.Extension.Shared.Dto
{
    public interface IDpdtBindingTarget
    {
        Guid BindingIdentifier
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
