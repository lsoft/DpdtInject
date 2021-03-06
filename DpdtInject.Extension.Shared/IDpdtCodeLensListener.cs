using System;
using System.Threading.Tasks;
using DpdtInject.Extension.Shared.Dto;

namespace DpdtInject.Extension.Shared
{
    public interface IDpdtCodeLensListener
    {
        Task<bool> IsEnabled(
            );

        Task<DpdtBindingReferenceSet> GetReferenceSet(
            CodeLensTarget target
            );

        int GetVisualStudioPid();
    }
}
