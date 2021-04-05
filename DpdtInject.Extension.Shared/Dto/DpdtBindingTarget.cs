using System;

namespace DpdtInject.Extension.Shared.Dto
{
    public class DpdtBindingTarget : IDpdtBindingTarget
    {
        public Guid BindingIdentifier
        {
            get;
        }

        public IDpdtClusterDetail ClusterDetail
        {
            get;
            set;
        }

        public IDpdtBindingDetail BindingDetail
        {
            get;
            set;
        }

        public DpdtBindingTarget(
            Guid bindingIdentifier,
            DpdtClusterDetail clusterDetail,
            DpdtBindingDetail bindingDetail
            )
        {
            if (clusterDetail is null)
            {
                throw new ArgumentNullException(nameof(clusterDetail));
            }

            if (bindingDetail is null)
            {
                throw new ArgumentNullException(nameof(bindingDetail));
            }

            BindingIdentifier = bindingIdentifier;
            ClusterDetail = clusterDetail;
            BindingDetail = bindingDetail;
        }

    }
}