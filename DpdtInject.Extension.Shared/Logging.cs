using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DpdtInject.Extension.Shared
{
    /// <summary>
    /// Logging helpers.
    /// Taken from  https://github.com/bert2/microscope completely.
    /// Take a look to that repo, it's amazing!
    /// </summary>
    public static class Logging
    {
        private static readonly object @lock = new object();

        // Logs go to: C:\Users\<user>\AppData\Local\Temp\dpdt_extension.*.log
        // We're using one log file for each process to prevent concurrent file access.
        private static readonly string vsLogFile = $"{Path.GetTempPath()}/dpdt_extension.vs.log";
        private static readonly string clLogFile = $"{Path.GetTempPath()}/dpdt_extension.codelens.log";

        [Conditional("DEBUG")]
        public static void LogVS(
            object? data = null,
            [CallerFilePath] string? file = null,
            [CallerMemberName] string? method = null)
            => Log(vsLogFile, file!, method!, data);

        [Conditional("DEBUG")]
        public static void LogCL(
            object? data = null,
            [CallerFilePath] string? file = null,
            [CallerMemberName] string? method = null)
            => Log(clLogFile, file!, method!, data);

        public static void Log(
            string logFile,
            string callingFile,
            string callingMethod,
            object? data = null)
        {
            lock (@lock)
            {
                File.AppendAllText(
                    logFile,
                    $"{DateTime.Now:HH:mm:ss.fff} "
                    + $"{Process.GetCurrentProcess().Id,5} "
                    + $"{Thread.CurrentThread.ManagedThreadId,3} "
                    + $"{Path.GetFileNameWithoutExtension(callingFile)}.{callingMethod}()"
                    + $"{(data == null ? "" : $": {data}")}\n");
            }
        }
    }
}
