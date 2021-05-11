using System;

namespace DpdtInject.Extension.Shared.Dto
{
    public class DpdtBindingTarget : IDpdtBindingTarget
    {
        public DpdtBindingIdentifier BindingIdentifier
        {
            get;
        }

        public Guid BindingUniqueUnstableIdentifier => BindingIdentifier.BindingUniqueUnstableIdentifier;

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
            DpdtBindingIdentifier bindingIdentifier,
            DpdtClusterDetail clusterDetail,
            DpdtBindingDetail bindingDetail
            )
        {
            if (bindingIdentifier is null)
            {
                throw new ArgumentNullException(nameof(bindingIdentifier));
            }

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