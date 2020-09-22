using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingsContainer
    {
        private readonly List<IBindingContainer> _bindingContainers;
        private readonly BindingClusterTree _bindingClusterTree;

        public IReadOnlyList<IBindingContainer> BindingContainers => _bindingContainers;

        public BindingClusterTree BindingClusterTree => _bindingClusterTree;

        public BindingsContainer(
            TreeJoint<ITypeSymbol?> declaredClusterJoint,
            List<IBindingContainer> bindingContainers
            )
        {
            if (declaredClusterJoint is null)
            {
                throw new ArgumentNullException(nameof(declaredClusterJoint));
            }

            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            _bindingContainers = bindingContainers;

            _bindingClusterTree = new BindingClusterTree(
                declaredClusterJoint.ConvertTo<BindingClusterJoint, BindingContainerCluster>(
                   joint => new BindingClusterJoint(
                       new BindingContainerCluster(
                            joint.JointPayload,
                            bindingContainers.FindAll(c => SymbolEqualityComparer.Default.Equals(c.DeclaredClusterType, joint.JointPayload))
                            )
                       )
                   )
                );
        }
    }

}
