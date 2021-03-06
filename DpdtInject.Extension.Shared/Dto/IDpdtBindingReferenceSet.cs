using System.Collections.Generic;

namespace DpdtInject.Extension.Shared.Dto
{
    public interface IDpdtBindingReferenceSet
    {
        CodeLensTarget Target
        {
            get;
        }

        IReadOnlyList<IDpdtBindingTarget> BindingTargets
        {
            get;
        }
    }
}
