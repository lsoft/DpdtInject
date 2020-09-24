using DpdtInject.Injector.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector
{
    public class DTimer : IDisposable
    {
        private readonly IDiagnosticReporter _diagnosticReporter;
        private readonly string _message;

        private readonly DateTime _before;
        private DateTime _after;

        public TimeSpan Diff => _after - _before;

        public DTimer(
            IDiagnosticReporter diagnosticReporter,
            string message
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _diagnosticReporter = diagnosticReporter;
            _message = message;
            
            _before = DateTime.Now;
            _after = _before;
        }

        public void Dispose()
        {
            _after = DateTime.Now;

            _diagnosticReporter.ReportInfo(
                _message,
                $"{_message}: {Diff}"
                );
        }
    }
}
