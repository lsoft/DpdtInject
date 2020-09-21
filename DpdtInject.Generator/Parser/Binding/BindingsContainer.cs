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
            TreeJoint<string> clusterNameJoint,
            List<IBindingContainer> bindingContainers
            )
        {
            if (clusterNameJoint is null)
            {
                throw new ArgumentNullException(nameof(clusterNameJoint));
            }

            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            _bindingContainers = bindingContainers;

            _bindingClusterTree = new BindingClusterTree(
                clusterNameJoint.ConvertTo(
                   clusterName => new BindingContainerCluster(
                        clusterName,
                        bindingContainers.FindAll(c => c.Name == clusterName)
                        )
                   )
                );
        }

        public void BuildFlags(
            IDiagnosticReporter diagnosticReporter
            )
        {
            if (diagnosticReporter is null)
            {
                throw new ArgumentNullException(nameof(diagnosticReporter));
            }

            _bindingClusterTree.BuildFlags(
                diagnosticReporter
                );
        }
    }

}
