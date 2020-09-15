using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator
{
    public interface IDiagnosticReporter
    {
        void ReportError(
            string title,
            string message
            );

        void ReportWarning(
            string title,
            string message
            );
    }

}
