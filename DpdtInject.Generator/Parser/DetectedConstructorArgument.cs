using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Cluster;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Generator.Producer.RContext;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Parser
{
    public class DetectedConstructorArgument
    {
        public string Name
        {
            get;
        }

        public ITypeSymbol? Type
        {
            get;
        }
        
        public string? Body
        {
            get;
        }

        public bool HasExplicitDefaultValue
        {
            get;
        }

        public bool DefineInBindNode => !string.IsNullOrEmpty(Body);

        public DetectedConstructorArgument(
            string name,
            string body
            )
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            Name = name;
            Type = null;
            Body = body;
            HasExplicitDefaultValue = false;
        }

        public DetectedConstructorArgument(
            string name,
            ITypeSymbol type,
            bool hasExplicitDefaultValue
            )
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Name = name;
            Type = type;
            Body = null;
            HasExplicitDefaultValue = hasExplicitDefaultValue;
        }

        public string GenerateProvideConstructorArgumentMethod(
            ClusterGeneratorTreeJoint clusterGeneratorJoint,
            IBindingContainer bindingContainer
            )
        {
            if (clusterGeneratorJoint is null)
            {
                throw new ArgumentNullException(nameof(clusterGeneratorJoint));
            }

            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            //DefineInBindNode should be checked BEFORE than HasExplicitDefaultValue
            if (DefineInBindNode)
            {
                return string.Empty;
            }
            if(HasExplicitDefaultValue)
            {
                return string.Empty;
            }


            var localVariableContextReference = "resolutionContext";

            var applyArgumentPiece = string.Empty;

            if(Type is null)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Type is null somehow");
            }
            
            var workingType = this.Type;
            DpdtArgumentWrapperTypeEnum wrapperType = DpdtArgumentWrapperTypeEnum.None;

            var registeredKeys = new List<(DpdtArgumentWrapperTypeEnum, ITypeSymbol)>();
            if (!clusterGeneratorJoint.TryGetRegisteredKeys(workingType, false, ref registeredKeys))
            {
                //this type is not registered in the current cluster and its parent clusters
                if (!workingType.TryDetectWrapperType(out wrapperType, out var innerType))
                {
                    //no, it's not a wrapper
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.NoBindingAvailable,
                        $"No bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]",
                        workingType.GetFullName()
                        );
                }

                //it's a wrapper
            }

            var pairs = new List<ClusterPair>();
            if (!clusterGeneratorJoint.TryGetPairs(workingType, true, ref pairs))
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]",
                    workingType.GetFullName()
                    );
            }

            var exceptionSuffix =
                pairs.Count > 1
                    ? ", but conditional bindings exists"
                    : string.Empty
                    ;

            var createFrameClause = "";

            #region ResolutionFrame variables

            var createFrameVariableNameDict = new Dictionary<string, string>();
            var createContextWithFrameVariableNameDict = new Dictionary<string, string>();

            foreach (var pair in pairs)
            {
                var createFrameVariableName = $"Frame_{workingType.GetFullName().EscapeSpecialTypeSymbols()}_{pair.InstanceContainerGenerator.BindingContainer.BindToType.GetFullName().EscapeSpecialTypeSymbols()}_{Name}_{pair.InstanceContainerGenerator.GetVariableStableName()}";
                createFrameVariableNameDict[pair.InstanceContainerGenerator.GetVariableStableName()] = createFrameVariableName;
                
                var contextWithFrameVariableName = $"contextWithFrame_{createFrameVariableName}";
                createContextWithFrameVariableNameDict[pair.InstanceContainerGenerator.GetVariableStableName()] = contextWithFrameVariableName;
            }


            foreach (var pair in pairs)
            {
                var createFrameVariableName = createFrameVariableNameDict[pair.InstanceContainerGenerator.GetVariableStableName()];

                createFrameClause += $@"
private static readonly {nameof(ResolutionFrame)} {createFrameVariableName} =
    {ResolutionFrameGenerator.GetNewFrameClause(pair.Joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name, workingType.GetFullName(), pair.InstanceContainerGenerator.BindingContainer.BindToType.GetFullName(), Name)};
";
            }

            #endregion


            if (pairs.Count == 0)
            {
                applyArgumentPiece = $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {workingType.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]{exceptionSuffix}", workingType.GetFullName())}
}}
";
            }
            else if (pairs.Count == 1)
            {
                var pair0 = pairs[0];

                if (pair0.InstanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional)
                {
                    var createFrameVariableName = createFrameVariableNameDict[pair0.InstanceContainerGenerator.GetVariableStableName()];

                    applyArgumentPiece = $@"
{createFrameClause}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {workingType.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    var context = {localVariableContextReference}.{nameof(ResolutionContext.AddFrame)}({createFrameVariableName});

    if({pair0.InstanceContainerGenerator.ClassName}.CheckPredicate(context))
    {{
        return {pair0.InstanceContainerGenerator.GetInstanceClause(pair0.Joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name, "context", wrapperType)};
    }}

    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]{exceptionSuffix}", workingType.GetFullName())}
}}
";
                }
                else
                {
                    applyArgumentPiece = $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {workingType.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    return {pair0.InstanceContainerGenerator.GetInstanceClause(pair0.Joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name, "null", wrapperType)};
}}
";
                }
            }
            else
            {
                if (pairs.Count(p => !p.InstanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional) > 1)
                {
                    applyArgumentPiece = $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {workingType.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]", workingType.GetFullName())}
}}
";
                }
                else
                {
                    var nonConditionalGeneratorCount = pairs.Count(p => !p.InstanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional);

                    applyArgumentPiece = $@"
{createFrameClause}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {workingType.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    int allowedChildrenCount = {nonConditionalGeneratorCount};
";

                    foreach (var pair in pairs)
                    {
                        if (pair.InstanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional)
                        {
                            var createFrameVariableName = createFrameVariableNameDict[pair.InstanceContainerGenerator.GetVariableStableName()];
                            var createContextWithFrameVariableName = createContextWithFrameVariableNameDict[pair.InstanceContainerGenerator.GetVariableStableName()];

                            applyArgumentPiece += $@"
var {createContextWithFrameVariableName} = {localVariableContextReference}.{nameof(ResolutionContext.AddFrame)}({createFrameVariableName});
";

                            applyArgumentPiece += $@"
var {pair.InstanceContainerGenerator.GetVariableStableName()} = false;
if({pair.InstanceContainerGenerator.ClassName}.CheckPredicate({createContextWithFrameVariableName}))
{{
    if(++allowedChildrenCount > 1)
    {{
        {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]", workingType.GetFullName())}
    }}

    {pair.InstanceContainerGenerator.GetVariableStableName()} = true;
}}
";
                        }
                    }

                    applyArgumentPiece += $@"
if(allowedChildrenCount == 0)
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]{exceptionSuffix}", workingType.GetFullName())}
}}
";

                    for(var pIndex = 0; pIndex < pairs.Count; pIndex++)
                    {
                        var pair = pairs[pIndex];
                        var isLast = pIndex == (pairs.Count - 1);

                        if (pair.InstanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional && !isLast)
                        {
                            var createContextWithFrameVariableName = createContextWithFrameVariableNameDict[pair.InstanceContainerGenerator.GetVariableStableName()];

                            applyArgumentPiece += $@"
if({pair.InstanceContainerGenerator.GetVariableStableName()})
{{
    return {pair.InstanceContainerGenerator.GetInstanceClause(pair.Joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name, createContextWithFrameVariableName, wrapperType)};
}}
";
                        }
                        else
                        {
                            applyArgumentPiece += $@"
return {pair.InstanceContainerGenerator.GetInstanceClause(pair.Joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name, "null", wrapperType)};
";
                        }
                    }

                    applyArgumentPiece += $@"
}}
";
                }
            }

            return applyArgumentPiece;
        }

        public string GetApplyConstructorClause(
            )
        {
            //DefineInBindNode should be checked BEFORE than HasExplicitDefaultValue
            if (DefineInBindNode)
            {
                return $"{Name}: {Body}";
            }
            if (HasExplicitDefaultValue)
            {
                return $"";
            }

            return $"{Name}: Get_{Name}(resolutionContext)";
        }

    }
}
