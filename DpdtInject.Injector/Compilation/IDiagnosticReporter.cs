namespace DpdtInject.Injector.Compilation
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
