using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
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
                (TreeJoint<ClusterGenerator> joint) =>
                {
                    var item = joint.JointPayload.GenerateClusterBody();
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
                    var item = $"private readonly {joint.JointPayload.ClusterClassName} {joint.JointPayload.ClusterStableInstanceName};";
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
{joint.JointPayload.ClusterStableInstanceName} = (({nameof(IClusterProvider<object>)}<{joint.JointPayload.ClusterClassName}>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
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
                    var i = $"{nameof(IClusterProvider<object>)}<{joint.JointPayload.ClusterClassName}>";
                    interfaces.Add(i);

                    var m = $@"
{joint.JointPayload.ClusterClassName} {nameof(IClusterProvider<object>)}<{joint.JointPayload.ClusterClassName}>.{nameof(IClusterProvider<object>.GetCluster)}()
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
                    var item = $"private readonly {joint.JointPayload.ClusterClassName} {joint.JointPayload.ClusterStableInstanceName};";
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
                    var item = $@"
{joint.JointPayload.ClusterStableInstanceName} = new {joint.JointPayload.ClusterClassName}(
    );
";
                    result.AppendLine(item);
                }
                );

            return result.ToString();
        }

    }

    public class ClusterGeneratorTreeJoint : TreeJoint<ClusterGenerator>
    {
        public ClusterGeneratorTreeJoint(ClusterGenerator jointPayload)
            : base(jointPayload)
        {
        }
    }

}
