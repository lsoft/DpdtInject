using Microsoft.VisualStudio.Shell.Interop;

namespace DpdtInject.Extension
{
    public interface IVsSolutionEventsExt : IVsSolutionEvents
    {
        uint Cookie
        {
            get;
            set;
        }
    }
}
