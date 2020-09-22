using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
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

namespace DpdtInject.Generator.Producer.Blocks.Cluster
{
    public class ClusterInterfaceGenerator
    {
        private readonly IReadOnlyList<InstanceContainerGenerator> _instanceContainerGenerators;

        public ITypeSymbol BindFromType
        {
            get;
        }

        public DpdtArgumentWrapperTypeEnum WrapperType
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

        public string GetExplicitImplementationSection
        {
            get;
        } = string.Empty;

        public string GetAllImplementationSection
        {
            get;
        } = string.Empty;
        public string GetAllExplicitImplementationSection
        {
            get;
        } = string.Empty;

        public ClusterInterfaceGenerator(
            ITypeSymbol bindFromType,
            DpdtArgumentWrapperTypeEnum wrapperType,
            IReadOnlyList<InstanceContainerGenerator> instanceContainerGenerators
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
            WrapperType = wrapperType;
            BindFromTypeFullName = bindFromType.GetFullName();
            _instanceContainerGenerators = instanceContainerGenerators;

            #region InterfaceSection 

            InterfaceSection = $"{nameof(IClusterProvider<object>)}<{BindFromTypeFullName}>";

            #endregion

            var exceptionSuffix =
                instanceContainerGenerators.Count > 1
                    ? ", but conditional bindings exists"
                    : string.Empty
                    ;

            var emptyContextReference = $"{typeof(ResolutionContext).FullName}.{nameof(ResolutionContext.EmptyContext)}";

            var createContextClause = "";

            #region ResolutionContext variables

            var createContextVariableNameDict = new Dictionary<string, string>();

            foreach (var generator in _instanceContainerGenerators)
            {
                var createContextVariableName = $"Context_{BindFromTypeFullName.ConvertDotLessGreatherToGround()}_{generator.BindingContainer.BindToType.GetFullName().ConvertDotLessGreatherToGround()}_{Guid.NewGuid().RemoveMinuses()}";
                createContextVariableNameDict[generator.GetVariableStableName()] = createContextVariableName;
            }

            #endregion

            foreach (var generator in _instanceContainerGenerators)
            {
                var createContextVariableName = createContextVariableNameDict[generator.GetVariableStableName()];

                createContextClause += $@"
private static readonly {nameof(ResolutionContext)} {createContextVariableName} = {emptyContextReference}.{nameof(ResolutionContext.AddFrame)}(
    {ResolutionFrameGenerator.GetNewFrameClause(BindFromTypeFullName, generator.BindingContainer.BindToType.GetFullName())}
    );
";
            }

            #region ResolutionFrameSection

            ResolutionFrameSection = createContextClause;

            #endregion

            var getImplementationMethodName = $"Get_{BindFromTypeFullName.ConvertDotLessGreatherToGround()}{wrapperType.GetPostfix()}";

            #region GetGenericImplementationSection

            GetExplicitImplementationSection = $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
{BindFromTypeFullName} {nameof(IClusterProvider<object>)}<{BindFromTypeFullName}>.Get()
{{
    return {getImplementationMethodName}();
}}
";

            #endregion

            #region GetImplementationSection

            if (instanceContainerGenerators.Count == 0)
            {
                GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
public {BindFromTypeFullName} {getImplementationMethodName}()
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
                    var createContextVariableName = createContextVariableNameDict[instanceContainerGenerator.GetVariableStableName()];

                    GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
public {BindFromTypeFullName} {getImplementationMethodName}()
{{
    if({instanceContainerGenerator.ClassName}.CheckPredicate({createContextVariableName}))
    {{
        return {instanceContainerGenerator.GetInstanceClause(createContextVariableName, WrapperType)};
    }}

    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{BindFromTypeFullName}]{exceptionSuffix}", BindFromTypeFullName)}
}}
";
                }
                else
                {
                    GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
public {BindFromTypeFullName} {getImplementationMethodName}()
{{
    return {instanceContainerGenerator.GetInstanceClause("null", WrapperType)};
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
public {BindFromTypeFullName} {getImplementationMethodName}()
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings available for [{BindFromTypeFullName}]", BindFromTypeFullName)}
}}
";
                }
                else
                {
                    var nonConditionalGeneratorCount = instanceContainerGenerators.Count(g => !g.BindingContainer.IsConditional);

                    GetImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
public {BindFromTypeFullName} {getImplementationMethodName}()
{{
    int allowedChildrenCount = {nonConditionalGeneratorCount};
";


                    foreach (var conditionalGenerator in instanceContainerGenerators.Where(g => g.BindingContainer.IsConditional))
                    {
                        var createContextVariableName = createContextVariableNameDict[conditionalGenerator.GetVariableStableName()];

                        GetImplementationSection += $@"
var {conditionalGenerator.GetVariableStableName()} = false;
if({conditionalGenerator.ClassName}.CheckPredicate({createContextVariableName}))
{{
    if(++allowedChildrenCount > 1)
    {{
        {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings available for [{BindFromTypeFullName}]", BindFromTypeFullName)}
    }}

    {conditionalGenerator.GetVariableStableName()} = true;
}}
";

                    }

                    GetImplementationSection += $@"
if(allowedChildrenCount == 0)
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{BindFromTypeFullName}]{exceptionSuffix}", BindFromTypeFullName)}
}}
";

                    for (var gIndex = 0; gIndex < instanceContainerGenerators.Count; gIndex++)
                    {
                        var generator = instanceContainerGenerators[gIndex];
                        var isLastGenerator = gIndex == instanceContainerGenerators.Count - 1;

                        if (generator.ItselfOrAtLeastOneChildIsConditional && !isLastGenerator)
                        {
                            var createContextVariableName = createContextVariableNameDict[generator.GetVariableStableName()];

                            GetImplementationSection += $@"
if({generator.GetVariableStableName()})
{{
    return {generator.GetInstanceClause(createContextVariableName, WrapperType)};
}}
";
                        }
                        else
                        {
                            GetImplementationSection += $@"
return {generator.GetInstanceClause("null", WrapperType)};
";
                        }
                    }

                    GetImplementationSection += $@"
}}
";
                }
            }

            #endregion

            var getAllImplementationMethodName = $"GetAll_{BindFromTypeFullName.ConvertDotLessGreatherToGround()}{wrapperType.GetPostfix()}";

            #region GetAllImplementation

            GetAllImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
public IEnumerable<object> {getAllImplementationMethodName}()
{{
    return (({nameof(IClusterProvider<object>)}<{BindFromTypeFullName}>)this).GetAll();
}}
";

            #endregion

            #region GetAllExplicitImplementationSection
            {
                GetAllExplicitImplementationSection = $@"
//[MethodImpl(MethodImplOptions.AggressiveInlining)]
List<{BindFromTypeFullName}> {nameof(IClusterProvider<object>)}<{BindFromTypeFullName}>.GetAll()
{{
    var result = new List<{BindFromTypeFullName}>();
";

                //var contextClauseApplied = false;
                foreach (var instanceContainerGenerator in instanceContainerGenerators)
                {
                    if (instanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional)
                    {
                        var createContextVariableName = createContextVariableNameDict[instanceContainerGenerator.GetVariableStableName()];

                        GetAllExplicitImplementationSection += $@"
    if({instanceContainerGenerator.ClassName}.CheckPredicate({createContextVariableName}))
    {{
        result.Add( {instanceContainerGenerator.GetInstanceClause(createContextVariableName, WrapperType)} );
    }}
";
                    }
                    else
                    {
                        GetAllExplicitImplementationSection += $@"
    result.Add( {instanceContainerGenerator.GetInstanceClause("null", WrapperType)} );
";
                    }
                }

                GetAllExplicitImplementationSection += $@"

    return result;
}}
";
            }

            #endregion
        }
    }

}
