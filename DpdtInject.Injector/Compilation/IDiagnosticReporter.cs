namespace DpdtInject.Injector.Compilation
{
    public interface IDiagnosticReporter
    {
        void ReportWarning(
            string title,
            string message
            );
    }

}
