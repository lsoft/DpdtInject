using System.Collections.Generic;

namespace DpdtInject.Injector.Module.RContext
{
    public interface IResolutionContext
    {
        IReadOnlyList<IResolutionFrame> Frames { get; }


        bool IsRoot { get; }

        IResolutionFrame RootFrame { get; }

        IResolutionFrame CurrentFrame { get; }

    }
}