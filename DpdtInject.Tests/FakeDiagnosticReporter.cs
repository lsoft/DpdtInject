using DpdtInject.Generator.Reporter;
using DpdtInject.Injector.Compilation;
using System.Diagnostics;

namespace DpdtInject.Tests
{
    internal class FakeDiagnosticReporter : IDiagnosticReporter
    {
        public int ErrorCount
        {
            get;
            private set;
        }

        public int WarningCount
        {
            get;
            private set;
        }


        public FakeDiagnosticReporter()
        {
            ErrorCount = 0;
            WarningCount = 0;
        }

        public void ReportError(string title, string message)
        {
            Debug.WriteLine(title);
            Debug.WriteLine(message);
            Debug.WriteLine(string.Empty);
            ErrorCount++;
        }

        public void ReportWarning(string title, string message)
        {
            Debug.WriteLine(title);
            Debug.WriteLine(message);
            Debug.WriteLine(string.Empty);
            WarningCount++;
        }
    }
}