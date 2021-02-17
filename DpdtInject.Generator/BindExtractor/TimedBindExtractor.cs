using DpdtInject.Generator.Binding;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace DpdtInject.Generator.BindExtractor
{
    public class TimedBindExtractor
    {
        private readonly IDiagnosticReporter _diagnosticReporter;
        private readonly DefaultBindExtractor _bindExtractor;

        public TimedBindExtractor(
            IDiagnosticReporter diagnosticReporter,
            DefaultBindExtractor bindExtractor
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

        public ClusterBindings GetClusterBindings(ITypeSymbol clusterType)
        {
            using (new DTimer(_diagnosticReporter, "Dpdt get bindings container taken"))
            {
                return _bindExtractor.GetClusterBindings(clusterType);
            }
        }
    }
}