using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Producer.Blocks.Provider
{
    public class ProviderGenerator
    {
        private readonly List<ProviderInterfaceGenerator> _interfaceSection;
        private readonly Compilation _compilation;

        public InstanceContainerGeneratorsContainer Container
        {
            get;
        }


        public IReadOnlyList<ProviderInterfaceGenerator> InterfaceSection => _interfaceSection;
        
        public string ProviderClassName
        {
            get;
        }


        public ProviderGenerator(
            Compilation compilation,
            InstanceContainerGeneratorsContainer container
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            _compilation = compilation;
            Container = container;

            _interfaceSection = new List<ProviderInterfaceGenerator>();

            foreach (var (_, bindFromType) in Container.Groups.GetRegisteredKeys(false))
            {
                if(Container.Groups.TryGetRegisteredGenerators(bindFromType, false, out var generators))
                {
                    _interfaceSection.Add(
                        new ProviderInterfaceGenerator(bindFromType, DpdtArgumentWrapperTypeEnum.None, generators)
                        );

                    foreach (var (wrapperType, key) in bindFromType.GenerateWrapperTypes(_compilation))
                    {
                        _interfaceSection.Add(
                            new ProviderInterfaceGenerator(key, wrapperType, generators)
                            );
                    }
                }
            }

            ProviderClassName = $"{nameof(Provider)}{this.GetHashCode()}";
        }

        public string GenerateProviderBody(
            InstanceContainerGeneratorsContainer container
            )
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return $@"
private class {ProviderClassName} : IDisposable
    {GetCombinedInterfaces()}
{{

    public {ProviderClassName}()
    {{
    }}

    public void Dispose()
    {{
        {container.InstanceContainerGenerators.Where(icg => icg.BindingContainer.Scope.In(BindScopeEnum.Singleton)).Join(sc => sc.DisposeClause + ";")}
    }}

    {GetCombinedImplementationSection()}

#region Instance Containers
    {container.InstanceContainerGenerators.Join(sc => sc.GetClassBody(container))}
#endregion

}}
";
        }

        private string GetCombinedInterfaces()
        {
            if (_interfaceSection.Count == 0)
            {
                return string.Empty;
            }

            return "," + string.Join(",", InterfaceSection.Select(j => j.InterfaceSection));
        }

        private string GetCombinedImplementationSection()
        {
            if (_interfaceSection.Count == 0)
            {
                return string.Empty;
            }

            return
                string.Join(Environment.NewLine, InterfaceSection.Select(i => i.ResolutionFrameSection))
                + string.Join(
                    Environment.NewLine,
                    InterfaceSection.Select(j =>
                        j.GetExplicitImplementationSection
                        + Environment.NewLine
                        + j.GetImplementationSection
                        + Environment.NewLine
                        + j.GetAllImplementationSection
                        + Environment.NewLine
                        + j.GetAllExplicitImplementationSection
                        )
                    )
                ;
        }

    }

}
