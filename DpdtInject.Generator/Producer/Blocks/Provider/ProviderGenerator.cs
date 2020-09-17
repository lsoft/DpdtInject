using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
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
            InstanceContainerGeneratorsContainer container
            )
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            Container = container;

            _interfaceSection = new List<ProviderInterfaceGenerator>();

            foreach (var pair in Container.Groups.ContainerGroups)
            {
                var bindFromType = pair.Key;
                var containers = pair.Value;


                _interfaceSection.Add(
                    new ProviderInterfaceGenerator(bindFromType, containers)
                    );
            }
        }

    }

}
