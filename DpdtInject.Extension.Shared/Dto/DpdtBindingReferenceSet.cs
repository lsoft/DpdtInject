using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Extension.Shared.Dto
{
    public class DpdtBindingReferenceSet : IDpdtBindingReferenceSet
    {
        public DpdtBindingReferenceSetStatusEnum Status
        {
            get;
            set;
        }

        /// <inheritdoc />
        public CodeLensTarget Target
        {
            get;
        }

        public IReadOnlyList<DpdtBindingTarget> BindingTargets
        {
            get;
            set;
        }

        IReadOnlyList<IDpdtBindingTarget> IDpdtBindingReferenceSet.BindingTargets => BindingTargets;

        public DpdtBindingReferenceSet(
            DpdtBindingReferenceSetStatusEnum status,
            CodeLensTarget target,
            IReadOnlyList<DpdtBindingTarget> bindingTargets
            )
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (bindingTargets is null)
            {
                throw new ArgumentNullException(nameof(bindingTargets));
            }

            Status = status;
            Target = target;
            BindingTargets = bindingTargets;
        }

        public int GetBindingCount()
        {
            return BindingTargets.Sum(m => m.BindingDetail != null ? 1 : 0);
        }


        public static DpdtBindingReferenceSet GetWithResults(
            CodeLensTarget target,
            IReadOnlyList<DpdtBindingTarget> bindingTargets
            )
        {
            return new DpdtBindingReferenceSet(
                DpdtBindingReferenceSetStatusEnum.Processed,
                target,
                bindingTargets
                );
        }

        public static DpdtBindingReferenceSet GetInProgress(CodeLensTarget target)
        {
            return new DpdtBindingReferenceSet(
                DpdtBindingReferenceSetStatusEnum.InProgress,
                target,
                new List<DpdtBindingTarget>()
                );
        }

        public static DpdtBindingReferenceSet GetDisabled(CodeLensTarget target)
        {
            return new DpdtBindingReferenceSet(
                DpdtBindingReferenceSetStatusEnum.Disabled,
                target,
                new List<DpdtBindingTarget>()
                );
        }
    }
}
