using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Extension.Shared.Dto
{
    public enum DpdtBindingReferenceSetStatusEnum
    {
        Disabled,
        InProgress,
        Processed
    }

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


    public class DpdtClusterDetail : IDpdtClusterDetail
    {
        public string ClassNamespace
        {
            get;
            set;
        }

        public string ClassFullName
        {
            get;
            set;
        }

        public string MethodName
        {
            get;
            set;
        }


        public string FullName => $"{ClassFullName}.{MethodName}";

        public DpdtClusterDetail(
            string classNamespace,
            string classFullName,
            string methodName
            )
        {
            if (classNamespace is null)
            {
                throw new ArgumentNullException(nameof(classNamespace));
            }

            if (classFullName is null)
            {
                throw new ArgumentNullException(nameof(classFullName));
            }

            if (methodName is null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }
            ClassNamespace = classNamespace;
            ClassFullName = classFullName;
            MethodName = methodName;
        }
    }

    public class DpdtBindingDetail : IDpdtBindingDetail
    {
        public string BindScope
        {
            get;
            set;
        }

        public bool ConditionalBinding
        {
            get;
            set;
        }

        public DpdtBindingDetail(
            string bindScope,
            bool conditionalBinding
            )
        {
            BindScope = bindScope;
            ConditionalBinding = conditionalBinding;
        }
    }

}
