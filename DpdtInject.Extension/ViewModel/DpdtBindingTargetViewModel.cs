using System;
using System.Collections.Generic;
using System.Windows.Threading;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;

namespace DpdtInject.Extension.ViewModel
{
    public class DpdtBindingReferenceSetViewModel : BaseViewModel, IDpdtBindingReferenceSet
    {
        public CodeLensTarget Target
        {
            get;
        }

        private readonly DpdtBindingReferenceSet _set;


        /// <inheritdoc />
        public IReadOnlyList<IDpdtBindingTarget> BindingTargets
        {
            get;
        }

        /// <inheritdoc />
        public DpdtBindingReferenceSetViewModel(
            Dispatcher dispatcher,
            DpdtBindingReferenceSet set
            )
            : base(dispatcher)
        {
            if (set is null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            _set = set;
            Target = set.Target;

            var bindingTargets = new List<IDpdtBindingTarget>();
            foreach (var bindingTarget in set.BindingTargets)
            {
                bindingTargets.Add(
                    new DpdtBindingTargetViewModel(
                        dispatcher,
                        bindingTarget
                        )
                    );
            }

            BindingTargets = bindingTargets;
        }

    }

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

    public class DpdtBindingDetailViewModel : BaseViewModel, IDpdtBindingDetail
    {
        private readonly IDpdtBindingDetail _target;

        /// <inheritdoc />
        public string BindScope
        {
            get;
        }

        /// <inheritdoc />
        public bool ConditionalBinding
        {
            get;
        }

        public DpdtBindingDetailViewModel(
            Dispatcher dispatcher,
            IDpdtBindingDetail target
            ) : base(dispatcher)
        {
            _target = target;

            BindScope = target.BindScope;
            ConditionalBinding = target.ConditionalBinding;
        }

    }

    public class DpdtClusterDetailViewModel : BaseViewModel, IDpdtClusterDetail
    {
        private readonly IDpdtClusterDetail _target;

        /// <inheritdoc />
        public string ClassFullName
        {
            get;
        }

        /// <inheritdoc />
        public string MethodName
        {
            get;
        }

        /// <inheritdoc />
        public string FullName
        {
            get;
        }

        public DpdtClusterDetailViewModel(
            Dispatcher dispatcher,
            IDpdtClusterDetail target
            ) : base(dispatcher)
        {
            _target = target;

            ClassFullName = target.ClassFullName;
            MethodName = target.MethodName;
            FullName = target.FullName;
        }

    }
}
