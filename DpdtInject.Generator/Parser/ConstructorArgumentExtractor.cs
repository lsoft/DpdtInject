using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Generator.Producer.RContext;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
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

        public string GetRetrieveConstructorArgumentClause(
            InstanceContainerGeneratorsContainer container,
            BindingContainer bindingContainer
            )
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
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

            var instanceContainerGenerators = container.Groups.ContainerGroups[this.Type];

            var exceptionSuffix =
                instanceContainerGenerators.Count > 1
                    ? ", but conditional bindings exists"
                    : string.Empty
                    ;

            var createFrameVariableName = $"Context_{Type.GetFullName().ConvertDotToGround()}_{Name}";

            var createFrameClause = $@"
private static readonly {nameof(ResolutionFrame)} {createFrameVariableName} = {ResolutionFrameGenerator.GetNewFrameClause(Type.GetFullName(), Name)};
";

            if (instanceContainerGenerators.Count == 0)
            {
                applyArgumentPiece = $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {Type.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings [{Type.GetFullName()}] available for [{bindingContainer.TargetTypeFullName}]{exceptionSuffix}", Type.GetFullName())}
}}
";
            }
            else if (instanceContainerGenerators.Count == 1)
            {
                var instanceContainerGenerator = instanceContainerGenerators[0];

                if (instanceContainerGenerator.ItselfOrAtLeastOneChildIsConditional)
                {
                    applyArgumentPiece = $@"
{createFrameClause}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {Type.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    var context = {localVariableContextReference}.{nameof(ResolutionContext.AddFrame)}({createFrameVariableName});

    if({instanceContainerGenerator.ClassName}.CheckPredicate(context))
    {{
        return {instanceContainerGenerator.GetInstanceClause("context")};
    }}

    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings [{Type.GetFullName()}] available for [{bindingContainer.TargetTypeFullName}]{exceptionSuffix}", Type.GetFullName())}
}}
";
                }
                else
                {
                    applyArgumentPiece = $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {Type.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
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
                    applyArgumentPiece = $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {Type.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings [{Type.GetFullName()}] available for [{bindingContainer.TargetTypeFullName}]", Type.GetFullName())}
}}
";
                }
                else
                {
                    var nonConditionalGeneratorCount = instanceContainerGenerators.Count(g => !g.BindingContainer.IsConditional);

                    applyArgumentPiece = $@"
{createFrameClause}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private static {Type.GetFullName()} Get_{Name}({nameof(ResolutionContext)} {localVariableContextReference})
{{
    int allowedChildrenCount = {nonConditionalGeneratorCount};
";

                    var contextClauseApplied = false;
                    foreach (var conditionalGenerator in instanceContainerGenerators.Where(g => g.BindingContainer.IsConditional))
                    {
                        if (conditionalGenerator.ItselfOrAtLeastOneChildIsConditional)
                        {
                            if (!contextClauseApplied)
                            {
                                applyArgumentPiece += $@"
var context = {localVariableContextReference}.{nameof(ResolutionContext.AddFrame)}({createFrameVariableName});
";
                                contextClauseApplied = true;
                            }
                        }

                        applyArgumentPiece += $@"
var {conditionalGenerator.GetVariableStableName()} = false;
if({conditionalGenerator.ClassName}.CheckPredicate(context))
{{
    if(++allowedChildrenCount > 1)
    {{
        {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings [{Type.GetFullName()}] available for [{bindingContainer.TargetTypeFullName}]", Type.GetFullName())}
    }}

    {conditionalGenerator.GetVariableStableName()} = true;
}}
";
                        
                    }

                    applyArgumentPiece += $@"
if(allowedChildrenCount == 0)
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings [{Type.GetFullName()}] available for [{bindingContainer.TargetTypeFullName}]{exceptionSuffix}", Type.GetFullName())}
}}
";

                    for(var gIndex = 0; gIndex < instanceContainerGenerators.Count; gIndex++)
                    {
                        var generator = instanceContainerGenerators[gIndex];
                        var isLastGenerator = gIndex == (instanceContainerGenerators.Count - 1);

                        if (generator.ItselfOrAtLeastOneChildIsConditional && !isLastGenerator)
                        {
                            applyArgumentPiece += $@"
if({generator.GetVariableStableName()})
{{
    return {generator.GetInstanceClause("context")};
}}
";
                        }
                        else
                        {
                            applyArgumentPiece += $@"
return {generator.GetInstanceClause("null")};
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
            InstanceContainerGeneratorsContainer container
            )
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (DefineInBindNode)
            {
                return $"{Name}: {Body}";
            }

            return $"{Name}: Get_{Name}(resolutionContext)";
        }

    }
}
