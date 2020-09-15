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

        public InstanceContainerGeneratorGroups ProviderGroups
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

                return string.Join(Environment.NewLine, InterfaceSection.Select(j => j.GetImplementationSection + Environment.NewLine + j.GetAllImplementationSection));
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

            //_providerGroups = new Dictionary<string, List<BindingContainer>>();

            //foreach (var bindingProcessor in container.BindingContainers)
            //{
            //    foreach (var bindFromType in bindingProcessor.BindFromTypes)
            //    {
            //        var key = bindFromType.GetFullName();
            //        if (!_providerGroups.ContainsKey(key))
            //        {
            //            _providerGroups[key] = new List<BindingContainer>();
            //        }

            //        _providerGroups[key].Add(bindingProcessor);
            //    }
            //}

            ProviderGroups = container.ConvertToGroups();

            _interfaceSection = new List<ProviderInterfaceGenerator>();

            foreach (var pair in ProviderGroups.ContainerGroups)
            {
                _interfaceSection.Add(
                    new ProviderInterfaceGenerator(pair.Key.GetFullName(), pair.Value)
                    );
            }
        }

    }

    public class ProviderInterfaceGenerator
    {
        private readonly List<InstanceContainerGenerator> _instanceContainerGenerators;

        public string BindFromTypeFullName
        {
            get;
        }

        public IReadOnlyList<InstanceContainerGenerator> InstanceContainerGenerators => _instanceContainerGenerators;

        public string InterfaceSection
        {
            get;
        } = string.Empty;

        public string GetImplementationSection
        {
            get;
        } = string.Empty;

        public string GetAllImplementationSection
        {
            get;
        } = string.Empty;

        public ProviderInterfaceGenerator(
            string bindFromTypeFullName,
            List<InstanceContainerGenerator> instanceContainerGenerators
            )
        {
            if (bindFromTypeFullName is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypeFullName));
            }

            if (instanceContainerGenerators is null)
            {
                throw new ArgumentNullException(nameof(instanceContainerGenerators));
            }

            BindFromTypeFullName = bindFromTypeFullName;
            _instanceContainerGenerators = instanceContainerGenerators;

            InterfaceSection = $"{nameof(IBaseProvider<object>)}<{BindFromTypeFullName}>";

            if (instanceContainerGenerators.Count == 1)
            {
                GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{bindFromTypeFullName} IBaseProvider<{bindFromTypeFullName}>.Get()
{{
    //TODO сделать выборку изо всех контейнеров и учесть предикат
    return {instanceContainerGenerators[0].ClassName}.GetInstance();
}}
";
            }
            else
            {
                GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{bindFromTypeFullName} IBaseProvider<{bindFromTypeFullName}>.Get()
{{
    //TODO сделать выборку изо всех контейнеров и учесть предикат
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, "Too many bindings availble", bindFromTypeFullName)}
}}
";
            }

            GetAllImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
List<{bindFromTypeFullName}> IBaseProvider<{bindFromTypeFullName}>.GetAll()
{{
    return
        new List<{bindFromTypeFullName}>
        {{
            //TODO учесть предикат у каждого контейнера
            {string.Join(",", instanceContainerGenerators.Select(b => $"{b.ClassName}.GetInstance()"))}
        }};
}}
";

        }
    }

}
