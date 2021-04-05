using System;

namespace DpdtInject.Extension.Shared
{
    /// <summary>
    /// Taken from  https://github.com/bert2/microscope completely.
    /// Take a look to that repo, it's amazing!
    /// </summary>
    public static class CodeLensPipeName
    {
        // Pipe needs to be scoped by PID so multiple VS instances don't compete for connecting CodeLenses.
        public static string Get(int pid) => $@"DpdtVisualStudioCodeLens\{pid}";
    }

    public interface IRemoteCodeLens
    {
        void Refresh();
    }

    public interface IRemoteVisualStudioCodeLens
    {
        void RegisterCodeLensDataPoint(Guid id);
    }
}
