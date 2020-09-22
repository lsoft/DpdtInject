using DpdtInject.Generator.Tree;
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
                    var clusterBody = joint.JointPayload.GenerateClusterBody();
                    result.AppendLine(clusterBody);
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
