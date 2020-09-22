using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
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
            InstanceContainerGeneratorCluster cluster,
            IBindingContainer bindingContainer
            )
        {
            if (cluster is null)
            {
                throw new ArgumentNullException(nameof(cluster));
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

            DpdtArgumentWrapperTypeEnum wrapperType = DpdtArgumentWrapperTypeEnum.None;
            if (cluster.GetRegisteredKeys(false).All(p => !SymbolEqualityComparer.Default.Equals(p.Item2, this.Type)))
            {
                //this type is not registered in the module
                if (!this.Type.TryDetectWrapperType(out wrapperType, out var innerType))
                {
                    //no, it's not a wrapper
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.NoBindingAvailable,
                        $"No bindings [{Type.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]",
                        Type.GetFullName()
                        );
                }

                //it's a wrapper
            }

            var workingType = this.Type;

            if(!cluster.TryGetRegisteredGeneratorGroups(workingType, true, out var groups))
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings [{Type.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]",
                    Type.GetFullName()
                    );
            }

            var generators = groups.Collapse(
                group => group.Generators
                );

            var exceptionSuffix =
                generators.Count > 1
                    ? ", but conditional bindings exists"
                    : string.Empty
                    ;

            var createFrameClause = "";

            #region ResolutionFrame variables

            var createFrameVariableNameDict = new Dictionary<string, string>();
            var createContextWithFrameVariableNameDict = new Dictionary<string, string>();

            foreach (var generator in generators)
            {
                var createFrameVariableName = $"Frame_{workingType.GetFullName().ConvertDotLessGreatherToGround()}_{generator.BindingContainer.BindToType.GetFullName().ConvertDotLessGreatherToGround()}_{Name}_{generator.GetVariableStableName()}";
                createFrameVariableNameDict[generator.GetVariableStableName()] = createFrameVariableName;
                
                var contextWithFrameVariableName = $"contextWithFrame_{createFrameVariableName}";
                createContextWithFrameVariableNameDict[generator.GetVariableStableName()] = contextWithFrameVariableName;
            }


            foreach (var generator in generators)
            {
                var createFrameVariableName = createFrameVariableNameDict[generator.GetVariableStableName()];

                createFrameClause += $@"
private static readonly {nameof(ResolutionFrame)} {createFrameVariableName} =
    {ResolutionFrameGenerator.GetNewFrameClause(workingType.GetFullName(), generator.BindingContainer.BindToType.GetFullName(), Name)};
";
            }

            #endregion


            if (generators.Count == 0)
            {
                applyArgumentPiece = $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {workingType.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]{exceptionSuffix}", workingType.GetFullName())}
}}
";
            }
            else if (generators.Count == 1)
            {
                var instanceContainerGenerator = generators[0];

                if (instanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional)
                {
                    var createFrameVariableName = createFrameVariableNameDict[instanceContainerGenerator.GetVariableStableName()];

                    applyArgumentPiece = $@"
{createFrameClause}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {workingType.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    var context = {localVariableContextReference}.{nameof(ResolutionContext.AddFrame)}({createFrameVariableName});

    if({instanceContainerGenerator.ClassName}.CheckPredicate(context))
    {{
        return {instanceContainerGenerator.GetInstanceClause("context", wrapperType)};
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
    return {instanceContainerGenerator.GetInstanceClause("null", wrapperType)};
}}
";
                }
            }
            else
            {
                if (generators.Count(cg => !cg.BindingContainer.IsConditional) > 1)
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
                    var nonConditionalGeneratorCount = generators.Count(g => !g.ItselfOrAtLeastOneChildIsConditional);

                    applyArgumentPiece = $@"
{createFrameClause}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {workingType.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    int allowedChildrenCount = {nonConditionalGeneratorCount};
";

                    //var contextClauseApplied = false;
                    foreach (var generator in generators)//.Where(g => g.BindingContainer.IsConditional))
                    {
                        if (generator.ItselfOrAtLeastOneChildIsConditional)
                        {
                            var createFrameVariableName = createFrameVariableNameDict[generator.GetVariableStableName()];
                            var createContextWithFrameVariableName = createContextWithFrameVariableNameDict[generator.GetVariableStableName()];

                            applyArgumentPiece += $@"
var {createContextWithFrameVariableName} = {localVariableContextReference}.{nameof(ResolutionContext.AddFrame)}({createFrameVariableName});
";

                            applyArgumentPiece += $@"
var {generator.GetVariableStableName()} = false;
if({generator.ClassName}.CheckPredicate({createContextWithFrameVariableName}))
{{
    if(++allowedChildrenCount > 1)
    {{
        {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]", workingType.GetFullName())}
    }}

    {generator.GetVariableStableName()} = true;
}}
";
                        }
                        else
                        {
                            applyArgumentPiece += $@"
var {generator.GetVariableStableName()} = false;
if({generator.ClassName}.CheckPredicate(null))
{{
    if(++allowedChildrenCount > 1)
    {{
        {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings [{workingType.GetFullName()}] available for [{bindingContainer.TargetRepresentation}]", workingType.GetFullName())}
    }}

    {generator.GetVariableStableName()} = true;
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

                    for(var gIndex = 0; gIndex < generators.Count; gIndex++)
                    {
                        var generator = generators[gIndex];
                        var isLastGenerator = gIndex == (generators.Count - 1);

                        if (generator.ItselfOrAtLeastOneChildIsConditional && !isLastGenerator)
                        {
                            var createContextWithFrameVariableName = createContextWithFrameVariableNameDict[generator.GetVariableStableName()];

                            applyArgumentPiece += $@"
if({generator.GetVariableStableName()})
{{
    return {generator.GetInstanceClause(createContextWithFrameVariableName, wrapperType)};
}}
";
                        }
                        else
                        {
                            applyArgumentPiece += $@"
return {generator.GetInstanceClause("null", wrapperType)};
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
