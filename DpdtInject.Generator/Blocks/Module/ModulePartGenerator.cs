using DpdtInject.Generator.Blocks.Binding;
using DpdtInject.Generator.Blocks.Provider;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Blocks.Module
{
    internal class ModulePartGenerator
    {
        private readonly IDiagnosticReporter _diagnosticReporter;

        public INamedTypeSymbol ModuleSymbol
        {
            get;
        }

        public string ModuleTypeNamespace => ModuleSymbol.ContainingNamespace.GetFullName();

        public string ModuleTypeName => ModuleSymbol.Name;

        public ModulePartGenerator(
            IDiagnosticReporter diagnosticReporter,
            INamedTypeSymbol moduleSymbol
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            if (moduleSymbol is null)
            {
                throw new ArgumentNullException(nameof(moduleSymbol));
            }
            _diagnosticReporter = diagnosticReporter;
            ModuleSymbol = moduleSymbol;
        }

        public string GetClassBody(
            BindingProcessorContainer bindingProcessorContainer
            )
        {
            if (bindingProcessorContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingProcessorContainer));
            }

            bindingProcessorContainer.AnalyzeForCircularDependencies(_diagnosticReporter);

            var providerGenerator = new ProviderGenerator(
                bindingProcessorContainer
                );

            var result = @$"
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;

{bindingProcessorContainer.BindingProcessors.Join(sc => sc.InstanceContainerGenerator.Usings.Join(c => c))}

namespace {ModuleTypeNamespace}
{{
#nullable enable
    public partial class {ModuleTypeName} : {nameof(DpdtModule)}
    {{
        private readonly Provider _provider = new Provider();


        public override void Dispose()
        {{
            {bindingProcessorContainer.BindingProcessors.Join(sc => sc.InstanceContainerGenerator.DisposeClause)}
        }}

        public T Get<T>()
        {{
            return ((IBaseProvider<T>)_provider).Get();
        }}


        public List<T> GetAll<T>()
        {{
            return ((IBaseProvider<T>)_provider).GetAll();
        }}

        private class Provider
            {providerGenerator.CombinedInterfaces}
        {{
            {providerGenerator.CombinedImplementationSection}
        }}
#nullable disable

    {bindingProcessorContainer.BindingProcessors.Join(sc => sc.InstanceContainerGenerator.GetClassBody(bindingProcessorContainer))}

    }}

}}
";

            return result;
        }

    }
}
