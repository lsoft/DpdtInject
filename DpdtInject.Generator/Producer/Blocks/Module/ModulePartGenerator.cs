using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Provider;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Module
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
            InstanceContainerGeneratorsContainer container
            )
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.BindingsContainer.AnalyzeForCircularDependencies(_diagnosticReporter);
            container.BindingsContainer.AnalyzeForSingletonTakesTransient(_diagnosticReporter);

            var providerGenerator = new ProviderGenerator(
                container
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

{container.InstanceContainerGenerators.Join(sc => sc.Usings.Join(c => c))}

namespace {ModuleTypeNamespace}
{{
#nullable enable
    public partial class {ModuleTypeName} : {nameof(DpdtModule)}
    {{
        private readonly Provider _provider = new Provider();


        public override void Dispose()
        {{
            {container.InstanceContainerGenerators.Where(icg => icg.BindingContainer.Scope.In(BindScopeEnum.Singleton)).Join(sc => sc.DisposeClause + ";")}
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

    {container.InstanceContainerGenerators.Join(sc => sc.GetClassBody(container))}

    }}

}}
";
            return result;
        }

    }
}
