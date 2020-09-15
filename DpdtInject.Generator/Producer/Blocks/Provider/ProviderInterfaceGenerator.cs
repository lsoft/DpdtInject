using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
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

            var exceptionSuffix =
                instanceContainerGenerators.Count > 1
                    ? ", but conditional bindings exists"
                    : string.Empty
                    ;

            var emptyContextReference = $"{nameof(ResolutionContext)}.{nameof(ResolutionContext.EmptyContext)}";

            if (instanceContainerGenerators.Count == 0)
            {
                GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{bindFromTypeFullName} IBaseProvider<{bindFromTypeFullName}>.Get()
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{bindFromTypeFullName}]{exceptionSuffix}", bindFromTypeFullName)}
}}
";
            }
            else if (instanceContainerGenerators.Count == 1)
            {
                GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{bindFromTypeFullName} IBaseProvider<{bindFromTypeFullName}>.Get()
{{
    if({instanceContainerGenerators[0].GetCheckPredicateClause(emptyContextReference)})
    {{
        return {instanceContainerGenerators[0].GetInstanceClause};
    }}

    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{bindFromTypeFullName}]{exceptionSuffix}", bindFromTypeFullName)}
}}
";
            }
            else
            {
                if (instanceContainerGenerators.Count(cg => !cg.AtLeastOneParentIsConditional) > 1)
                {
                    GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{bindFromTypeFullName} IBaseProvider<{bindFromTypeFullName}>.Get()
{{
            {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings available for [{bindFromTypeFullName}]", bindFromTypeFullName)}
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

    {bindFromTypeFullName} result = null;

    if({instanceContainerGenerators[0].GetCheckPredicateClause(emptyContextReference)})
    {{
        result = {instanceContainerGenerators[0].GetInstanceClause};
    }}
";

                    foreach (var instanceContainerGenerator in instanceContainerGenerators.Skip(1))
                    {
                        GetImplementationSection += $@"

    if({instanceContainerGenerator.GetCheckPredicateClause(emptyContextReference)})
    {{
        if(result is not null)
        {{
            {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings available for [{bindFromTypeFullName}]", bindFromTypeFullName)}
        }}

        result = {instanceContainerGenerator.GetInstanceClause};
    }}

";
                    }

                    GetImplementationSection += $@"
    if(result is null)
    {{
        {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{bindFromTypeFullName}]{exceptionSuffix}", bindFromTypeFullName)}
    }}

    return result;
}}
";
                }
            }

            GetAllImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
List<{bindFromTypeFullName}> IBaseProvider<{bindFromTypeFullName}>.GetAll()
{{
    var result = new List<{bindFromTypeFullName}>();
";

            foreach (var instanceContainerGenerator in instanceContainerGenerators)
            {
                GetAllImplementationSection += $@"

    if({instanceContainerGenerator.GetCheckPredicateClause(emptyContextReference)})
    {{
        result.Add( {instanceContainerGenerator.GetInstanceClause} );
    }}

";
            }

            GetAllImplementationSection += $@"

    return result;
}}
";

        }
    }

}
