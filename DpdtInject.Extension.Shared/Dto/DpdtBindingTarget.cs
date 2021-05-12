using System;

namespace DpdtInject.Extension.Shared.Dto
{
    public class DpdtBindingTarget : IDpdtBindingTarget
    {
        public Guid BindingUniqueUnstableIdentifier
        {
            get;
            set;
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
            Guid bindingUniqueUnstableIdentifier,
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

            BindingUniqueUnstableIdentifier = bindingUniqueUnstableIdentifier;
            ClusterDetail = clusterDetail;
            BindingDetail = bindingDetail;
        }

    }
}