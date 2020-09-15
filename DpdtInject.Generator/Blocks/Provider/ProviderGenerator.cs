using DpdtInject.Generator.Blocks.Binding;
using DpdtInject.Generator.Blocks.Exception;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Blocks.Provider
{
    public class ProviderGenerator
    {
        private readonly List<ProviderInterfaceGenerator> _interfaceSection;
        private readonly Dictionary<string, List<BindingProcessor>> _providerGroups;

        public IReadOnlyDictionary<string, List<BindingProcessor>> ProviderGroups => _providerGroups;

        public IReadOnlyList<ProviderInterfaceGenerator> InterfaceSection => _interfaceSection;

        public string CombinedInterfaces
        {
            get
            {
                if(_interfaceSection.Count == 0)
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
            BindingProcessorContainer bindingProcessorContainer
            )
        {
            _providerGroups = new Dictionary<string, List<BindingProcessor>>();

            foreach (var bindingProcessor in bindingProcessorContainer.BindingProcessors)
            {
                foreach (var bindFromType in bindingProcessor.BindFromTypes)
                {
                    var key = bindFromType.GetFullName();
                    if (!_providerGroups.ContainsKey(key))
                    {
                        _providerGroups[key] = new List<BindingProcessor>();
                    }

                    _providerGroups[key].Add(bindingProcessor);
                }
            }

            _interfaceSection = new List<ProviderInterfaceGenerator>();

            foreach(var pair in _providerGroups)
            {
                _interfaceSection.Add(
                    new ProviderInterfaceGenerator(pair.Key, pair.Value)
                    );
            }
        }

    }

    public class ProviderInterfaceGenerator
    {
        private readonly List<BindingProcessor> _bindingProcessors;

        public string BindFromTypeFullName
        {
            get;
        }

        public IReadOnlyList<BindingProcessor> BindingProcessors => _bindingProcessors;

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
            List<BindingProcessor> bindingProcessors
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
