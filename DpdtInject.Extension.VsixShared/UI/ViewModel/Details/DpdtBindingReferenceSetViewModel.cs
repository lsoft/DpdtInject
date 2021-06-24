using System;
using System.Collections.Generic;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;

namespace DpdtInject.Extension.UI.ViewModel.Details
{
    public class DpdtBindingReferenceSetViewModel : BaseViewModel, IDpdtBindingReferenceSet
    {
        private readonly DpdtBindingReferenceSet _set;

        public CodeLensTarget Target
        {
            get;
        }

        /// <inheritdoc />
        public IReadOnlyList<IDpdtBindingTarget> BindingTargets
        {
            get;
        }

        /// <inheritdoc />
        public DpdtBindingReferenceSetViewModel(
            DpdtBindingReferenceSet set
            )
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
                        bindingTarget
                        )
                    );
            }
            BindingTargets = bindingTargets;
        }

    }
}
