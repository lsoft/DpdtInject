using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
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
        private readonly Dictionary<string, List<BindingContainer>> _providerGroups;

        public IReadOnlyDictionary<string, List<BindingContainer>> ProviderGroups => _providerGroups;

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
            BindingsContainer bindingProcessorContainer
            )
        {
            _providerGroups = new Dictionary<string, List<BindingContainer>>();

            foreach (var bindingProcessor in bindingProcessorContainer.BindingProcessors)
            {
                foreach (var bindFromType in bindingProcessor.BindFromTypes)
                {
                    var key = bindFromType.GetFullName();
                    if (!_providerGroups.ContainsKey(key))
                    {
                        _providerGroups[key] = new List<BindingContainer>();
                    }

                    _providerGroups[key].Add(bindingProcessor);
                }
            }

            _interfaceSection = new List<ProviderInterfaceGenerator>();

            foreach (var pair in _providerGroups)
            {
                _interfaceSection.Add(
                    new ProviderInterfaceGenerator(pair.Key, pair.Value)
                    );
            }
        }

    }

    public class ProviderInterfaceGenerator
    {
        private readonly List<BindingContainer> _bindingProcessors;

        public string BindFromTypeFullName
        {
            get;
        }

        public IReadOnlyList<BindingContainer> BindingProcessors => _bindingProcessors;

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
            List<BindingContainer> bindingProcessors
            )
        {
            if (bindFromTypeFullName is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypeFullName));
            }

            if (bindingProcessors is null)
            {
                throw new ArgumentNullException(nameof(bindingProcessors));
            }

            BindFromTypeFullName = bindFromTypeFullName;
            _bindingProcessors = bindingProcessors;

            InterfaceSection = $"{nameof(IBaseProvider<object>)}<{BindFromTypeFullName}>";

            if (bindingProcessors.Count == 1)
            {
                GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{bindFromTypeFullName} IBaseProvider<{bindFromTypeFullName}>.Get()
{{
    //TODO сделать выборку изо всех контейнеров и учесть предикат
    return {bindingProcessors[0].InstanceContainerGenerator.ClassName}.GetInstance();
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
            {string.Join(",", bindingProcessors.Select(b => $"{b.InstanceContainerGenerator.ClassName}.GetInstance()"))}
        }};
}}
";

        }
    }

}
