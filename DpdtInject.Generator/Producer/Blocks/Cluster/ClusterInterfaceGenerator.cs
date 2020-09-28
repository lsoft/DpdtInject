using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Generator.Producer.RContext;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Cluster
{
    public class ClusterInterfaceGenerator : ISourceCodeGenerator
    {
        private readonly IReadOnlyList<InstanceContainerGenerator> _instanceContainerGenerators;
        private readonly EmbeddedScopeGenerator _embedded;
        private readonly CustomScopeGenerator _custom;

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


        public ClusterInterfaceGenerator(
            ClusterGenerator clusterGenerator,
            ITypeSymbol bindFromType,
            DpdtArgumentWrapperTypeEnum wrapperType,
            IReadOnlyList<InstanceContainerGenerator> instanceContainerGenerators
            )
        {
            if (clusterGenerator is null)
            {
                throw new ArgumentNullException(nameof(clusterGenerator));
            }

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

            InterfaceSection = $"{nameof(IBindingProvider<object>)}<{BindFromTypeFullName}>";

            #endregion

            _embedded = new EmbeddedScopeGenerator(
                clusterGenerator,
                bindFromType,
                wrapperType,
                instanceContainerGenerators
                );
            _custom = new CustomScopeGenerator(
                clusterGenerator,
                bindFromType,
                wrapperType,
                instanceContainerGenerators
                );
        }

        public string GetGeneratedCode()
        {
            return
                Environment.NewLine
                + "#region scopes"
                + Environment.NewLine
                + _embedded.GetGeneratedCode()
                + Environment.NewLine
                + _custom.GetGeneratedCode()
                + Environment.NewLine
                + "#endregion"
                ;
        }
    }

    public class CustomScopeGenerator : ISourceCodeGenerator
    {
        private readonly ImplementationGenerator _actualCodeContainer;

        public string GetGeneratedCode()
        {
            return
                Environment.NewLine
                + "#region custom scope"
                + Environment.NewLine
                + _actualCodeContainer.GetGeneratedCode()
                + Environment.NewLine
                + "#endregion"
                + Environment.NewLine
                ;
        }

        public CustomScopeGenerator(
            ClusterGenerator clusterGenerator,
            ITypeSymbol bindFromType,
            DpdtArgumentWrapperTypeEnum wrapperType,
            IReadOnlyList<InstanceContainerGenerator> instanceContainerGenerators
            )
        {
            if (clusterGenerator is null)
            {
                throw new ArgumentNullException(nameof(clusterGenerator));
            }

            if (bindFromType is null)
            {
                throw new ArgumentNullException(nameof(bindFromType));
            }

            if (instanceContainerGenerators is null)
            {
                throw new ArgumentNullException(nameof(instanceContainerGenerators));
            }

            const string contextArgumentName = "scope";

            _actualCodeContainer = new ImplementationGenerator(
                clusterGenerator,
                bindFromType,
                wrapperType,
                instanceContainerGenerators,
                new MethodArgument(nameof(CustomScopeObject), contextArgumentName),
                generator =>
                {
                    var contextVariableName = generator.GetContextStableName(bindFromType);
                    var scopedContextVariableName = $"scoped_{contextVariableName}";

                    var result = new ModifyContextClause(
                        SyntaxFactory.ParseExpression($@"
var {scopedContextVariableName} = {contextVariableName}.ModifyWith({contextArgumentName});
"),
                        scopedContextVariableName
                        );

                    return result;
                }
            );
        }

    }

    public class EmbeddedScopeGenerator : ISourceCodeGenerator
    {
        private readonly ImplementationGenerator _actualCodeContainer;

        public string ResolutionFrameSection
        {
            get;
        } = string.Empty;

        public string GetGeneratedCode()
        {
            return
                ResolutionFrameSection
                + Environment.NewLine
                + "#region embedded scopes"
                + Environment.NewLine
                + _actualCodeContainer.GetGeneratedCode()
                + Environment.NewLine
                + "#endregion"
                + Environment.NewLine
            ;
        }

        public EmbeddedScopeGenerator(
            ClusterGenerator clusterGenerator,
            ITypeSymbol bindFromType,
            DpdtArgumentWrapperTypeEnum wrapperType,
            IReadOnlyList<InstanceContainerGenerator> instanceContainerGenerators
            ) 
        {
            if (clusterGenerator is null)
            {
                throw new ArgumentNullException(nameof(clusterGenerator));
            }

            if (bindFromType is null)
            {
                throw new ArgumentNullException(nameof(bindFromType));
            }

            if (instanceContainerGenerators is null)
            {
                throw new ArgumentNullException(nameof(instanceContainerGenerators));
            }

            var exceptionSuffix =
                instanceContainerGenerators.Count > 1
                    ? ", but conditional bindings exists"
                    : string.Empty
                    ;

            #region ResolutionFrameSection

            var resolutionFrameSection = "";

            foreach (var generator in instanceContainerGenerators)
            {
                var contextVariableName = generator.GetContextStableName(bindFromType);

                resolutionFrameSection += $@"
private static readonly {nameof(ResolutionContext)} {contextVariableName} = new {nameof(ResolutionContext)}(
    {ResolutionFrameGenerator.GetNewFrameClause(clusterGenerator.Joint.JointPayload.DeclaredClusterType.Name, bindFromType.GetFullName(), generator.BindingContainer.BindToType.GetFullName())}
    );
";
            }

            ResolutionFrameSection = resolutionFrameSection;

            #endregion


            _actualCodeContainer = new ImplementationGenerator(
                clusterGenerator,
                bindFromType,
                wrapperType,
                instanceContainerGenerators,
                new MethodArgument(),
                generator =>
                {
                    var contextVariableName = generator.GetContextStableName(bindFromType);
                    var scopedContextVariableName = $"scoped_{contextVariableName}";

                    var result = new ModifyContextClause(
                        SyntaxFactory.ParseExpression($@"
var {scopedContextVariableName} = {contextVariableName};
"),
                        scopedContextVariableName
                        );

                    return result;
                }
            );
        }

    }

    public class MethodArgument
    {
        public string ArgumentType
        {
            get;
        }

        public string ArgumentName
        {
            get;
        }

        public MethodArgument()
        {
            ArgumentType = string.Empty;
            ArgumentName = string.Empty;
        }

        public MethodArgument(string argumentType, string argumentName)
        {
            ArgumentType = argumentType;
            ArgumentName = argumentName;
        }

        public override string ToString()
        {
            if(string.IsNullOrEmpty(ArgumentType) && string.IsNullOrEmpty(ArgumentName))
            {
                return string.Empty;
            }

            return $"{ArgumentType} {ArgumentName}";
        }
    }

    public class ModifyContextClause
    {
        public SyntaxNode AssignClause
        {
            get;
        }

        public string VariableName
        {
            get;
        }

        public ModifyContextClause(
            SyntaxNode assignClause,
            string variableName
            )
        {
            if (assignClause is null)
            {
                throw new ArgumentNullException(nameof(assignClause));
            }

            if (variableName is null)
            {
                throw new ArgumentNullException(nameof(variableName));
            }

            AssignClause = assignClause;
            VariableName = variableName;
        }
    }

    public class ImplementationGenerator : ISourceCodeGenerator
    {
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


        public ImplementationGenerator(
            ClusterGenerator clusterGenerator,
            ITypeSymbol bindFromType,
            DpdtArgumentWrapperTypeEnum wrapperType,
            IReadOnlyList<InstanceContainerGenerator> instanceContainerGenerators,
            MethodArgument methodArgument,
            Func<InstanceContainerGenerator, ModifyContextClause> modifyContextProvider
            )
        {
            if (clusterGenerator is null)
            {
                throw new ArgumentNullException(nameof(clusterGenerator));
            }

            if (bindFromType is null)
            {
                throw new ArgumentNullException(nameof(bindFromType));
            }

            if (instanceContainerGenerators is null)
            {
                throw new ArgumentNullException(nameof(instanceContainerGenerators));
            }

            if (methodArgument is null)
            {
                throw new ArgumentNullException(nameof(methodArgument));
            }

            if (modifyContextProvider is null)
            {
                throw new ArgumentNullException(nameof(modifyContextProvider));
            }

            var bindFromTypeFullName = bindFromType.GetFullName();

            var exceptionSuffix =
                instanceContainerGenerators.Count > 1
                    ? ", but conditional bindings exists"
                    : string.Empty
                    ;

            var getImplementationMethodName = $"Get_{bindFromTypeFullName.EscapeSpecialTypeSymbols()}{wrapperType.GetPostfix()}";

            #region GetGenericImplementationSection

            GetExplicitImplementationSection = $@"
[MethodImpl(MethodImplOptions.AggressiveInlining)]
{bindFromTypeFullName} {nameof(IBindingProvider<object>)}<{bindFromTypeFullName}>.Get({methodArgument})
{{
    return {getImplementationMethodName}({methodArgument.ArgumentName});
}}
";

            #endregion

            #region GetImplementationSection

            if (instanceContainerGenerators.Count == 0)
            {
                GetImplementationSection = $@"
public {bindFromTypeFullName} {getImplementationMethodName}({methodArgument})
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{bindFromTypeFullName}]{exceptionSuffix}", bindFromTypeFullName)}
}}
";
            }
            else if (instanceContainerGenerators.Count == 1)
            {
                var instanceContainerGenerator = instanceContainerGenerators[0];

                if (instanceContainerGenerator.NeedToProcessResolutionContext)
                {
                    var modifiedContext = modifyContextProvider(instanceContainerGenerator);

                    GetImplementationSection = $@"
public {bindFromTypeFullName} {getImplementationMethodName}({methodArgument})
{{
    {modifiedContext.AssignClause.GetText()}

    if({instanceContainerGenerator.ClassName}.CheckPredicate({modifiedContext.VariableName}))
    {{
        return {instanceContainerGenerator.GetInstanceClause(clusterGenerator.Joint.JointPayload.DeclaredClusterType.Name, modifiedContext.VariableName, wrapperType)};
    }}

    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{bindFromTypeFullName}]{exceptionSuffix}", bindFromTypeFullName)}
}}
";
                }
                else
                {
                    GetImplementationSection = $@"
public {bindFromTypeFullName} {getImplementationMethodName}({methodArgument})
{{
    return {instanceContainerGenerator.GetInstanceClause(clusterGenerator.Joint.JointPayload.DeclaredClusterType.Name, "null", wrapperType)};
}}
";
                }
            }
            else
            {
                if (instanceContainerGenerators.Count(cg => !cg.NeedToProcessResolutionContext) > 1)
                {
                    GetImplementationSection = $@"
public {bindFromTypeFullName} {getImplementationMethodName}({methodArgument})
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings available for [{bindFromTypeFullName}]", bindFromTypeFullName)}
}}
";
                }
                else
                {
                    var nonConditionalGeneratorCount = instanceContainerGenerators.Count(g => !g.NeedToProcessResolutionContext);

                    GetImplementationSection = $@"
public {bindFromTypeFullName} {getImplementationMethodName}({methodArgument})
{{
    int allowedChildrenCount = {nonConditionalGeneratorCount};
";


                    foreach (var conditionalGenerator in instanceContainerGenerators.Where(g => g.NeedToProcessResolutionContext))
                    {
                        var modifiedContext = modifyContextProvider(conditionalGenerator);

                        GetImplementationSection += $@"
var {conditionalGenerator.GetVariableStableName()} = false;

{modifiedContext.AssignClause.GetText()}

if({conditionalGenerator.ClassName}.CheckPredicate({modifiedContext.VariableName}))
{{
    if(++allowedChildrenCount > 1)
    {{
        {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.DuplicateBinding, $"Too many bindings available for [{bindFromTypeFullName}]", bindFromTypeFullName)}
    }}

    {conditionalGenerator.GetVariableStableName()} = true;
}}
";

                    }

                    GetImplementationSection += $@"
if(allowedChildrenCount == 0)
{{
    {ExceptionGenerator.GenerateThrowExceptionClause(DpdtExceptionTypeEnum.NoBindingAvailable, $"No bindings available for [{bindFromTypeFullName}]{exceptionSuffix}", bindFromTypeFullName)}
}}
";

                    for (var gIndex = 0; gIndex < instanceContainerGenerators.Count; gIndex++)
                    {
                        var generator = instanceContainerGenerators[gIndex];
                        var isLastGenerator = gIndex == instanceContainerGenerators.Count - 1;

                        if (generator.NeedToProcessResolutionContext && !isLastGenerator)
                        {
                            var modifiedContext = modifyContextProvider(generator);

                            GetImplementationSection += $@"
if({generator.GetVariableStableName()})
{{
    return {generator.GetInstanceClause(clusterGenerator.Joint.JointPayload.DeclaredClusterType.Name, modifiedContext.VariableName, wrapperType)};
}}
";
                        }
                        else
                        {
                            GetImplementationSection += $@"
return {generator.GetInstanceClause(clusterGenerator.Joint.JointPayload.DeclaredClusterType.Name, "null", wrapperType)};
";
                        }
                    }

                    GetImplementationSection += $@"
}}
";
                }
            }

            #endregion

            var getAllImplementationMethodName = $"GetAll_{bindFromTypeFullName.EscapeSpecialTypeSymbols()}{wrapperType.GetPostfix()}";

            #region GetAllImplementation

            GetAllImplementationSection = $@"
public IEnumerable<object> {getAllImplementationMethodName}({methodArgument})
{{
    return (({nameof(IBindingProvider<object>)}<{bindFromTypeFullName}>)this).GetAll({methodArgument.ArgumentName});
}}
";

            #endregion

            #region GetAllExplicitImplementationSection
            {
                GetAllExplicitImplementationSection = $@"
List<{bindFromTypeFullName}> {nameof(IBindingProvider<object>)}<{bindFromTypeFullName}>.GetAll({methodArgument})
{{
    var result = new List<{bindFromTypeFullName}>();
";

                foreach (var instanceContainerGenerator in instanceContainerGenerators)
                {
                    if (instanceContainerGenerator.NeedToProcessResolutionContext)
                    {
                        var modifiedContext = modifyContextProvider(instanceContainerGenerator);

                        GetAllExplicitImplementationSection += $@"
{modifiedContext.AssignClause.GetText()}

if({instanceContainerGenerator.ClassName}.CheckPredicate({modifiedContext.VariableName}))
{{
    result.Add( {instanceContainerGenerator.GetInstanceClause(clusterGenerator.Joint.JointPayload.DeclaredClusterType.Name, modifiedContext.VariableName, wrapperType)} );
}}
";
                    }
                    else
                    {
                        GetAllExplicitImplementationSection += $@"
result.Add( {instanceContainerGenerator.GetInstanceClause(clusterGenerator.Joint.JointPayload.DeclaredClusterType.Name, "null", wrapperType)} );
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

        public string GetGeneratedCode()
        {
            return
                GetExplicitImplementationSection
                + Environment.NewLine
                + GetImplementationSection
                + Environment.NewLine
                + GetAllImplementationSection
                + Environment.NewLine
                + GetAllExplicitImplementationSection
            ;
        }

    }

}
