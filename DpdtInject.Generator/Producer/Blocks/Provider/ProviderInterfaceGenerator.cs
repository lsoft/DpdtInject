using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Provider
{
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
