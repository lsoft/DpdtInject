using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Parser
{
    public class TimedBindExtractor
    {
        private readonly IDiagnosticReporter _diagnosticReporter;
        private readonly BindExtractor _bindExtractor;

        public TimedBindExtractor(
            IDiagnosticReporter diagnosticReporter,
            BindExtractor bindExtractor
            )
        {
            if (diagnosticReporter is null)
            {
                throw new System.ArgumentNullException(nameof(diagnosticReporter));
            }

            if (bindExtractor is null)
            {
                throw new System.ArgumentNullException(nameof(bindExtractor));
            }

            _diagnosticReporter = diagnosticReporter;
            _bindExtractor = bindExtractor;
        }

        public SyntaxNode? Visit(SyntaxNode? node)
        {
            using (new DTimer(_diagnosticReporter, "Dpdt parse bind clauses taken"))
            {
                return _bindExtractor.Visit(node);
            }
        }

        public BindingsContainer GetBindingsContainer(ITypeSymbol moduleType)
        {
            using (new DTimer(_diagnosticReporter, "Dpdt get bindings container taken"))
            {
                return _bindExtractor.GetBindingsContainer(moduleType);
            }
        }
    }
}