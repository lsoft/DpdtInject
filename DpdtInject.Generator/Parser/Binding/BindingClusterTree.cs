using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingClusterTree
    {
        public BindingClusterJoint ClusterJoint
        {
            get;
        }

        public BindingClusterTree(
            BindingClusterJoint clusterJoint
            )
        {
            if (clusterJoint is null)
            {
                throw new ArgumentNullException(nameof(clusterJoint));
            }

            ClusterJoint = clusterJoint;
        }
    }

    public class BindingClusterJoint : TreeJoint<BindingContainerCluster>
    {
        public BindingClusterJoint(TreeJoint<BindingContainerCluster>? parent, BindingContainerCluster jointPayload)
            : base(parent, jointPayload)
        {
        }
    }

}
