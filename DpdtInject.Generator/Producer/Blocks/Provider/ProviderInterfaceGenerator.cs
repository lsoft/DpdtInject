using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Generator.Producer.RContext;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Provider
{
    public class ProviderInterfaceGenerator
    {
        private readonly List<InstanceContainerGenerator> _instanceContainerGenerators;

        public ITypeSymbol BindFromType
        { 
            get;
        }

        public string BindFromTypeFullName
        {
            get;
        }

        public IReadOnlyList<InstanceContainerGenerator> InstanceContainerGenerators => _instanceContainerGenerators;

        public string InterfaceSection
        {
            get;
        } = string.Empty;

        public string ResolutionFrameSection
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
            ITypeSymbol bindFromType,
            List<InstanceContainerGenerator> instanceContainerGenerators
            )
        {
            if (bindFromType is null)
            {
                throw new ArgumentNullException(nameof(bindFromType));
            }

            if (instanceContainerGenerators is null)
            {
                throw new ArgumentNullException(nameof(instanceContainerGenerators));
            }

            BindFromType = bindFromType;
            BindFromTypeFullName = bindFromType.GetFullName();
            _instanceContainerGenerators = instanceContainerGenerators;

            InterfaceSection = $"{nameof(IBaseProvider<object>)}<{BindFromTypeFullName}>";

            var exceptionSuffix =
                instanceContainerGenerators.Count > 1
                    ? ", but conditional bindings exists"
                    : string.Empty
                    ;

            var emptyContextReference = $"{typeof(ResolutionContext).FullName}.{nameof(ResolutionContext.EmptyContext)}";

            var createContextVariableName = $"Context_{BindFromTypeFullName.ConvertDotToGround()}";

            var createContextClause = $@"
private static readonly {nameof(ResolutionContext)} {createContextVariableName} = {emptyContextReference}.{nameof(ResolutionContext.AddFrame)}(
    {ResolutionFrameGenerator.GetNewFrameClause(BindFromTypeFullName)}
    );
";

            #region ResolutionFrameSection

            ResolutionFrameSection = createContextClause;

            #endregion

            #region GetImplementationSection

            if (instanceContainerGenerators.Count == 0)
            {
                GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{BindFromTypeFullName} IBaseProvider<{BindFromTypeFullName}>.Get()
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{BindFromTypeFullName}]{exceptionSuffix}", BindFromTypeFullName)}
}}
";
            }
            else if (instanceContainerGenerators.Count == 1)
            {
                var instanceContainerGenerator = instanceContainerGenerators[0];

                if (instanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional)
                {
                    GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{BindFromTypeFullName} IBaseProvider<{BindFromTypeFullName}>.Get()
{{
    if({instanceContainerGenerator.ClassName}.CheckPredicate({createContextVariableName}))
    {{
        return {instanceContainerGenerator.GetInstanceClause(createContextVariableName)};
    }}

    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{BindFromTypeFullName}]{exceptionSuffix}", BindFromTypeFullName)}
}}
";
                }
                else
                {
                    GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{BindFromTypeFullName} IBaseProvider<{BindFromTypeFullName}>.Get()
{{
    return {instanceContainerGenerator.GetInstanceClause("null")};
}}
";
                }
            }
            else
            {
                if (instanceContainerGenerators.Count(cg => !cg.BindingContainer.IsConditional) > 1)
                {
                    GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{BindFromTypeFullName} IBaseProvider<{BindFromTypeFullName}>.Get()
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings available for [{BindFromTypeFullName}]", BindFromTypeFullName)}
}}
";
                }
                else
                {
                    GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
{BindFromTypeFullName} IBaseProvider<{BindFromTypeFullName}>.Get()
{{
    {BindFromTypeFullName} result = null;
";

                    //var contextClauseApplied = false;
                    foreach (var instanceContainerGenerator in instanceContainerGenerators)
                    {
                        if (instanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional)
                        {
                            //if(!contextClauseApplied)
                            //{
                            //    GetImplementationSection += createContextClause;
                            //    contextClauseApplied = true;
                            //}

                            GetImplementationSection += $@"
    if({instanceContainerGenerator.ClassName}.CheckPredicate({createContextVariableName}))
    {{
        if(!(result is null))
        {{
            {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings available for [{BindFromTypeFullName}]", BindFromTypeFullName)}
        }}

        result = {instanceContainerGenerator.GetInstanceClause(createContextVariableName)};
    }}

";
                        }
                        else
                        {
                            GetImplementationSection += $@"
    result = {instanceContainerGenerator.GetInstanceClause("null")};
";
                        }
                    }

                    GetImplementationSection += $@"
    if(result is null)
    {{
        {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{BindFromTypeFullName}]{exceptionSuffix}", BindFromTypeFullName)}
    }}

    return result;
}}
";
                }
            }

            #endregion

            #region GetAllImplementationSection
            {
                GetAllImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
List<{BindFromTypeFullName}> IBaseProvider<{BindFromTypeFullName}>.GetAll()
{{
    var result = new List<{BindFromTypeFullName}>();
";

                //var contextClauseApplied = false;
                foreach (var instanceContainerGenerator in instanceContainerGenerators)
                {
                    if (instanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional)
                    {
                        //if (!contextClauseApplied)
                        //{
                        //    GetAllImplementationSection += createContextClause;
                        //    contextClauseApplied = true;
                        //}


                        GetAllImplementationSection += $@"
    if({instanceContainerGenerator.ClassName}.CheckPredicate({createContextVariableName}))
    {{
        result.Add( {instanceContainerGenerator.GetInstanceClause(createContextVariableName)} );
    }}
";
                    }
                    else
                    {
                        GetAllImplementationSection += $@"
    result.Add( {instanceContainerGenerator.GetInstanceClause("null")} );
";
                    }
                }

                GetAllImplementationSection += $@"

    return result;
}}
";
            }

            #endregion
        }
    }

}
