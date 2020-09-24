using DpdtInject.Generator.Beautify;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector;
using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Producer.Blocks.Cluster
{
    public class ClusterGeneratorTree
    {
        public ClusterGeneratorTreeJoint Joint
        {
            get;
        }

        public ClusterGeneratorTree(
            ClusterGeneratorTreeJoint joint
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            Joint = joint;
        }

        public string GenerateClusterBodies()
        {
            var result = new StringBuilder();
            
            Joint.Apply(
                (TreeJoint<ClusterGenerator> jjoint) =>
                {
                    var joint = (ClusterGeneratorTreeJoint)jjoint;
                    
                    var item = joint.GenerateClusterBody(
                        );
                    result.AppendLine(item);
                }
                );

            return result.ToString();
        }

        public string GenerateUsingClauses()
        {
            var container = new HashSet<string>();

            Joint.Apply(
                (TreeJoint<ClusterGenerator> joint) =>
                {
                    var usings = joint.JointPayload.Joint.JointPayload.Generators.Join(sc => sc.Usings.Join(c => c));
                    container.Add(usings);
                }
                );

            return string.Join(Environment.NewLine, container);
        }

        public string GenerateClusterDeclarationClauses()
        {
            var result = new StringBuilder();

            result.AppendLine($"private readonly SuperCluster _superCluster;");

            Joint.Apply(
                (TreeJoint<ClusterGenerator> joint) =>
                {
                    var item = $"private readonly {joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name} {joint.JointPayload.ClusterStableInstanceName};";
                    result.AppendLine(item);
                }
                );

            return result.ToString();
        }

        public string GenerateClusterAssignClauses()
        {
            var result = new StringBuilder();

            result.AppendLine($@"
_superCluster = new SuperCluster();
");

            Joint.Apply(
                (TreeJoint<ClusterGenerator> joint) =>
                {
                    var item = $@"
{joint.JointPayload.ClusterStableInstanceName} = (({nameof(IClusterProvider<object>)}<{joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name}>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
";
                    result.AppendLine(item);
                }
                );

            return result.ToString();
        }

        public string GenerateClusterDisposeClauses()
        {
            var result = new StringBuilder();

            Joint.Apply(
                (TreeJoint<ClusterGenerator> joint) =>
                {
                    var item = $"{joint.JointPayload.ClusterStableInstanceName}.Dispose();";
                    result.AppendLine(item);
                }
                );

            return result.ToString();
        }

        public string GenerateSuperClusterBody()
        {
            var interfaces = new List<string>();
            var methods = new List<string>();
            Joint.Apply(
                (TreeJoint<ClusterGenerator> joint) =>
                {
                    var i = $"{nameof(IClusterProvider<object>)}<{joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name}>";
                    interfaces.Add(i);

                    var m = $@"
{joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name} {nameof(IClusterProvider<object>)}<{joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name}>.{nameof(IClusterProvider<object>.GetCluster)}()
{{
    return {joint.JointPayload.ClusterStableInstanceName};
}}
";
                    methods.Add(m);
                }
                );


            return $@"
private class SuperCluster :
    {string.Join(",", interfaces)}
{{
    {GenerateClusterDeclarationClausesPrivate()}

    public SuperCluster()
    {{
        {GenerateClusterAssignClausesPrivate()}

    }}

    {string.Join(Environment.NewLine, methods)}
}}
";
        }

        public string GenerateClusterDeclarationClausesPrivate()
        {
            var result = new StringBuilder();

            Joint.Apply(
                (TreeJoint<ClusterGenerator> joint) =>
                {
                    var item = $"private readonly {joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name} {joint.JointPayload.ClusterStableInstanceName};";
                    result.AppendLine(item);
                }
                );

            return result.ToString();
        }

        private string GenerateClusterAssignClausesPrivate()
        {
            var result = new StringBuilder();

            Joint.Apply(
                (TreeJoint<ClusterGenerator> joint) =>
                {
                    string item;
                    if (joint.IsRoot)
                    {
                        item = $@"
{joint.JointPayload.ClusterStableInstanceName} = new {joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name}(
    );
";
                    }
                    else
                    {
                        item = $@"
{joint.JointPayload.ClusterStableInstanceName} = new {joint.JointPayload.Joint.JointPayload.DeclaredClusterType.Name}(
    {joint.Parent!.JointPayload.ClusterStableInstanceName}
    );
";
                    }

                    result.AppendLine(item);
                }
                );

            return result.ToString();
        }

    }

    public class ClusterGeneratorTreeJoint : TreeJoint<ClusterGenerator>
    {
        public ClusterGeneratorTreeJoint(TreeJoint<ClusterGenerator>? parent, ClusterGenerator jointPayload)
            : base(parent, jointPayload)
        {
        }

        public bool TryGetRegisteredKeys(
            ITypeSymbol type,
            bool includeWrappers,
            ref List<(DpdtArgumentWrapperTypeEnum, ITypeSymbol)> result
            )
        {
            var instanceContainerCluster = JointPayload.Joint.JointPayload;

            var currentRegisteredKeys = instanceContainerCluster.GetRegisteredKeys(includeWrappers);
            
            result.AddRange(
                currentRegisteredKeys.Where(pair => SymbolEqualityComparer.Default.Equals(pair.Item2, type))
                );

            if (TryGetParent<ClusterGeneratorTreeJoint>(out var parent))
            {
                parent.TryGetRegisteredKeys(
                    type,
                    includeWrappers,
                    ref result
                    );
            }

            return result.Count > 0;
        }

        public bool TryGetPairs(
            ITypeSymbol type,
            bool includeWrappers,
            ref List<ClusterPair> result
            )
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var instanceContainerCluster = JointPayload.Joint.JointPayload;

            if (instanceContainerCluster.TryGetRegisteredGeneratorGroups(type, includeWrappers, out var groups))
            {
                var rresult = new List<InstanceContainerGenerator>();

                groups.ForEach(
                    group => rresult.AddRange(group.Generators)
                    );

                result.AddRange(
                    rresult.ConvertAll(a => new ClusterPair(this, a))
                    );
            }

            if (TryGetParent<ClusterGeneratorTreeJoint>(out var parent))
            {
                parent.TryGetPairs(
                    type,
                    includeWrappers,
                    ref result
                    );
            }

            return result.Count > 0;
        }

        public string GenerateClusterBody(
            )
        {
            var declaredClusterType = (INamedTypeSymbol)JointPayload.Joint.JointPayload.DeclaredClusterType;

            var beautifyGenerator = new BeautifyGenerator(
                declaredClusterType.Name
                );

            var fieldParentClusterClause = string.Empty;
            var constructorArgumentParentClusterClause = string.Empty;
            var assignParentClusterClause = string.Empty;
            var parameterlessConstructorClause = string.Empty;
            if (!IsRoot)
            {
                fieldParentClusterClause = $"private readonly {Parent!.JointPayload.Joint.JointPayload.DeclaredClusterType.Name} _parentCluster;";
                constructorArgumentParentClusterClause = $"{Parent!.JointPayload.Joint.JointPayload.DeclaredClusterType.Name} parentCluster";
                assignParentClusterClause = $"_parentCluster = parentCluster;";

                var parameterlessConstructor = declaredClusterType.InstanceConstructors.FirstOrDefault(c => c.Parameters.Length == 0);
                if (parameterlessConstructor?.IsImplicitlyDeclared ?? false)
                {
                    parameterlessConstructorClause = $@"
protected {declaredClusterType.Name}()
{{
}}
";
                }
            }

            return $@"
{declaredClusterType.DeclaredAccessibility.ToSource()} partial class {declaredClusterType.Name} : {nameof(IDisposable)}, {nameof(IBindingProvider)}
    {JointPayload.GetCombinedInterfaces()}
{{
    {fieldParentClusterClause}
    private readonly {beautifyGenerator.ClassName} _beautifier;

    public {typeof(ReinventedContainer).FullName} TypeContainerGet
    {{
        get;
    }}

    public {typeof(ReinventedContainer).FullName} TypeContainerGetAll
    {{
        get;
    }}

    public string DeclaredClusterType => ""{declaredClusterType.Name}"";

    public bool IsRootCluster => {((declaredClusterType.BaseType!.GetFullName() == "System.Object") ? "true" : "false")};

    public {typeof(IBeautifier).FullName} Beautifier => _beautifier;

    {parameterlessConstructorClause}

    public {declaredClusterType.Name}({constructorArgumentParentClusterClause})
    {{
        {assignParentClusterClause}

        TypeContainerGet = new {typeof(ReinventedContainer).FullName}(
            {JointPayload.Joint.JointPayload.GetReinventedContainerArgument("Get")}
            );
        TypeContainerGetAll = new {typeof(ReinventedContainer).FullName}(
            {JointPayload.Joint.JointPayload.GetReinventedContainerArgument("GetAll")}
            );

        _beautifier = new {beautifyGenerator.ClassName}(
            this
            );
    }}

    public bool IsRegisteredFrom<TRequestedType>()
    {{
        return this is {nameof(IBindingProvider<object>)}<TRequestedType>;
    }}

    public TRequestedType Get<TRequestedType>()
    {{
        return (({nameof(IBindingProvider<object>)}<TRequestedType>)this).Get();
    }}

    public List<TRequestedType> GetAll<TRequestedType>()
    {{
        return (({nameof(IBindingProvider<object>)}<TRequestedType>)this).GetAll();
    }}

    public object Get({typeof(Type).FullName} requestedType)
    {{
        var result = TypeContainerGet.{nameof(ReinventedContainer.GetGetObject)}(requestedType);

        return result;
    }}
    public IEnumerable<object> GetAll({typeof(Type).FullName} requestedType)
    {{
        var result = TypeContainerGetAll.{nameof(ReinventedContainer.GetGetObject)}(requestedType);

        return (IEnumerable<object>)result;
    }}

#region Beautify

    {beautifyGenerator.GenerateBeautifierBody()}

#endregion

    public void Dispose()
    {{
        {JointPayload.Joint.JointPayload.Generators.Where(icg => icg.BindingContainer.Scope.In(BindScopeEnum.Singleton)).Join(sc => sc.DisposeClause + ";")}
    }}

    {JointPayload.GetCombinedImplementationSection()}

#region Instance Containers
    {JointPayload.Joint.JointPayload.Generators.Join(sc => sc.GetClassBody(this))}
#endregion

}}
";
        }

    }

    public class ClusterPair
    {
        public ClusterGeneratorTreeJoint Joint 
        {
            get;
        }
        
        public InstanceContainerGenerator InstanceContainerGenerator
        {
            get;
        }

        public ClusterPair(
            ClusterGeneratorTreeJoint joint,
            InstanceContainerGenerator instanceContainerGenerator
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            if (instanceContainerGenerator is null)
            {
                throw new ArgumentNullException(nameof(instanceContainerGenerator));
            }

            Joint = joint;
            InstanceContainerGenerator = instanceContainerGenerator;
        }

    }

}
