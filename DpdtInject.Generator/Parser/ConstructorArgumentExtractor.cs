using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
using DpdtInject.Generator.Producer.Blocks.Cluster;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Generator.Producer.RContext;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Transactions;

namespace DpdtInject.Generator.Parser
{
    public class ConstructorArgumentExtractor : CSharpSyntaxRewriter
    {
        private readonly List<DetectedConstructorArgument> _constructorArguments;
        private readonly Compilation _compilation;
        private readonly SemanticModel _semanticModel;

        public ConstructorArgumentExtractor(
            Compilation compilation,
            SemanticModel semanticModel
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }
            _compilation = compilation;
            _semanticModel = semanticModel;

            _constructorArguments = new List<DetectedConstructorArgument>();
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (node.Type.ToString() != nameof(ConstructorArgument))
            {
                return base.VisitObjectCreationExpression(node)!;
            }
            if (node.ArgumentList == null || node.ArgumentList.Arguments == null || node.ArgumentList.Arguments.Count == 0)
            {
                return base.VisitObjectCreationExpression(node)!;
            }

            var constructorArgumentNameNode = node.ArgumentList.Arguments[0];
            if (!constructorArgumentNameNode.Expression.TryGetCompileTimeString(_semanticModel, out var argument))
            {
                throw new Exception(@"Constructor argument name should be direct-defined string or const string. Dpdt syntax parser does not support other options.");
            }

            var constructorArgumentBodyNode = node.ArgumentList.Arguments[1];
            var body = constructorArgumentBodyNode.ToString();

            _constructorArguments.Add(new DetectedConstructorArgument(argument, body));

            return base.VisitObjectCreationExpression(node)!;
        }

        internal List<DetectedConstructorArgument> GetConstructorArguments()
        {
            return new List<DetectedConstructorArgument>(_constructorArguments);
        }
    }

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
        }

        public DetectedConstructorArgument(
            string name,
            ITypeSymbol type
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



            if (DefineInBindNode)
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
                var createFrameVariableName = $"Frame_{workingType.GetFullName().ConvertDotLessGreatherToGround()}_{pair.InstanceContainerGenerator.BindingContainer.BindToType.GetFullName().ConvertDotLessGreatherToGround()}_{Name}_{pair.InstanceContainerGenerator.GetVariableStableName()}";
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
                if (pairs.Count(p => !p.InstanceContainerGenerator.BindingContainer.IsConditional) > 1)
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
            if (DefineInBindNode)
            {
                return $"{Name}: {Body}";
            }

            return $"{Name}: Get_{Name}(resolutionContext)";
        }

    }

    public static class ArgumentWrapperHelper
    {
        public static IEnumerable<(DpdtArgumentWrapperTypeEnum, ITypeSymbol)> GenerateWrapperTypes(
            this ITypeSymbol type,
            Compilation compilation
            )
        {
            foreach(DpdtArgumentWrapperTypeEnum wrapperType in Enum.GetValues(typeof(DpdtArgumentWrapperTypeEnum)))
            {
                INamedTypeSymbol wrapperSymbol;
                switch (wrapperType)
                {
                    case DpdtArgumentWrapperTypeEnum.None:
                        continue;
                    case DpdtArgumentWrapperTypeEnum.Func:
                        wrapperSymbol = compilation.GetTypeByMetadataName("System.Func`1")!;
                        wrapperSymbol = wrapperSymbol.Construct(type);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(wrapperType.ToString());
                }

                yield return (wrapperType, wrapperSymbol);
            }
        }

        public static string GetPostfix(
            this DpdtArgumentWrapperTypeEnum wrapperType
            )
        {
            switch (wrapperType)
            {
                case DpdtArgumentWrapperTypeEnum.None:
                    return string.Empty;
                case DpdtArgumentWrapperTypeEnum.Func:
                    return "_Func";
                default:
                    throw new ArgumentOutOfRangeException(wrapperType.ToString());
            }
        }

        public static bool TryDetectWrapperType(
            this ITypeSymbol type,
            out DpdtArgumentWrapperTypeEnum wrapperType,
            [NotNullWhen(true)] out ITypeSymbol? internalType
            )
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var namedType = (type as INamedTypeSymbol);
            if(namedType is null)
            {
                wrapperType = DpdtArgumentWrapperTypeEnum.None;
                internalType = null;
                return false;
            }

            var extractedName = type.Name;
            if(extractedName == "Func")
            {
                wrapperType = DpdtArgumentWrapperTypeEnum.Func;
                internalType = namedType.TypeArguments[0];
                return true;
            }

            wrapperType = DpdtArgumentWrapperTypeEnum.None;
            internalType = null;
            return false;
        }
    }

    public enum DpdtArgumentWrapperTypeEnum
    {
        None,
        Func
    }
}
