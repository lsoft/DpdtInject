using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
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

        public string CombinedInterfaces
        {
            get
            {
                if (_interfaceSection.Count == 0)
                {
                    return string.Empty;
                }

                return ":" + string.Join(",", InterfaceSection.Select(j => j.InterfaceSection));
            }
        }

        public string CombinedImplementationSection
        {
            get
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
        }

    }

}
