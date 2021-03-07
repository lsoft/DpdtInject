using System;
using System.Collections.Generic;
using System.Windows.Threading;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;
using Microsoft.VisualStudio.Imaging.Interop;

namespace DpdtInject.Extension.ViewModel.Details
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

        public ImageMoniker Logo
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

            Logo = new ImageMoniker
            {
                Guid = new Guid("bbd8a64b-7fd0-47fb-a600-503d90f22239"),
                Id = 0
            };

        }

    }
}
