using System;
using System.Windows.Threading;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;

namespace DpdtInject.Extension.ViewModel.Details
{
    public class DpdtBindingTargetViewModel : BaseViewModel, IDpdtBindingTarget
    {
        private readonly IDpdtBindingTarget _target;

        /// <inheritdoc />
        public Guid BindingIdentifier
        {
            get;
        }

        /// <inheritdoc />
        public IDpdtClusterDetail ClusterDetail
        {
            get;
        }

        /// <inheritdoc />
        public IDpdtBindingDetail BindingDetail
        {
            get;
        }

        /// <inheritdoc />
        public DpdtBindingTargetViewModel(
            Dispatcher dispatcher,
            IDpdtBindingTarget target
            )
            : base(dispatcher)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            _target = target;

            BindingIdentifier = target.BindingIdentifier;
            ClusterDetail = new DpdtClusterDetailViewModel(
                dispatcher,
                target.ClusterDetail
                );
            BindingDetail = new DpdtBindingDetailViewModel(
                dispatcher,
                target.BindingDetail
                );
        }

    }
}
