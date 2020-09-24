using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.Blocks.Module
{
    internal interface IModuleGenerator
    {
        INamedTypeSymbol ModuleSymbol
        {
            get;
        }
        
        string ModuleTypeName
        {
            get;
        }
        
        string ModuleTypeNamespace
        {
            get;
        }

        string GenerateModuleBody(IInstanceContainerGeneratorsContainer container);
    }

    internal class TimedModuleGenerator : IModuleGenerator
    {
        private readonly IDiagnosticReporter _diagnosticReporter;
        private readonly IModuleGenerator _moduleGenerator;

        public INamedTypeSymbol ModuleSymbol => _moduleGenerator.ModuleSymbol;

        public string ModuleTypeName => _moduleGenerator.ModuleTypeName;

        public string ModuleTypeNamespace => _moduleGenerator.ModuleTypeNamespace;

        public TimedModuleGenerator(
            IDiagnosticReporter diagnosticReporter,
            IModuleGenerator moduleGenerator
            )
        {
            if (diagnosticReporter is null)
            {
                throw new System.ArgumentNullException(nameof(diagnosticReporter));
            }

            if (moduleGenerator is null)
            {
                throw new System.ArgumentNullException(nameof(moduleGenerator));
            }

            _diagnosticReporter = diagnosticReporter;
            _moduleGenerator = moduleGenerator;
        }


        public string GenerateModuleBody(IInstanceContainerGeneratorsContainer container)
        {
            using (new DTimer(_diagnosticReporter, "Dpdt generate module body time taken"))
            {
                return _moduleGenerator.GenerateModuleBody(container);
            }
        }
    }
}