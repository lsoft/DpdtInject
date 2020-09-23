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
            ITypeSymbol moduleType,
            List<IBindingContainer> bindingContainers
            )
        {
            if (moduleType is null)
            {
                throw new ArgumentNullException(nameof(moduleType));
            }

            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            _bindingContainers = bindingContainers;

            //build declared cluster tree
            var declaredClusterTreeRoot = BuildDeclaredClusterTree(
                moduleType,
                bindingContainers
                );

            _bindingClusterTree = new BindingClusterTree(
                declaredClusterTreeRoot.ConvertTo2<BindingClusterJoint, BindingContainerCluster>(
                   (parentJoint, toConvertJoint) => new BindingClusterJoint(
                       parentJoint,
                       new BindingContainerCluster(
                            toConvertJoint.JointPayload,
                            bindingContainers.FindAll(c => SymbolEqualityComparer.Default.Equals(c.DeclaredClusterType, toConvertJoint.JointPayload))
                            )
                       )
                   )
                );
        }

        private TreeJoint<ITypeSymbol> BuildDeclaredClusterTree(
            ITypeSymbol moduleType,
            List<IBindingContainer> containers
            )
        {
            if (moduleType is null)
            {
                throw new ArgumentNullException(nameof(moduleType));
            }

            if (containers is null)
            {
                throw new ArgumentNullException(nameof(containers));
            }

            var declaredClusterTypes = containers
                .Select(c => c.DeclaredClusterType)
                .Where(dct => dct.BaseType!.GetFullName() == "System.Object")
                .Distinct()
                .ToList()
                ;

            if (declaredClusterTypes.Count > 1)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, "Too many root clusters");
            }

            var declaredClusterType = declaredClusterTypes[0];

            if (declaredClusterType.ContainingType is null
                || !SymbolEqualityComparer.Default.Equals(declaredClusterType.ContainingType, moduleType))
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.IncorrectCluster,
                    $"Cluster [{declaredClusterType.GetFullName()}] shound be nested in its parent module [{moduleType.GetFullName()}]",
                    declaredClusterType.GetFullName()
                    );
            }

            var rootJoint = new TreeJoint<ITypeSymbol>(
                null,
                declaredClusterType
                );

            BuildDeclaredClusterTree(
                moduleType,
                containers,
                rootJoint
                );

            return rootJoint;
        }


        private void BuildDeclaredClusterTree(
            ITypeSymbol moduleType,
            List<IBindingContainer> containers,
            TreeJoint<ITypeSymbol> joint
            )
        {
            if (moduleType is null)
            {
                throw new ArgumentNullException(nameof(moduleType));
            }

            if (containers is null)
            {
                throw new ArgumentNullException(nameof(containers));
            }

            var childContainers = containers.FindAll(c => SymbolEqualityComparer.Default.Equals(c.DeclaredClusterType.BaseType, joint.JointPayload));
            if (childContainers.Count > 0)
            {
                foreach (var childContainer in childContainers.Distinct())
                {
                    var declaredClusterType = childContainer.DeclaredClusterType;

                    if (declaredClusterType.ContainingType is null
                        || !SymbolEqualityComparer.Default.Equals(declaredClusterType.ContainingType, moduleType))
                    {
                        throw new DpdtException(
                            DpdtExceptionTypeEnum.IncorrectCluster,
                            $"Cluster [{declaredClusterType.GetFullName()}] shound be nested in its parent module [{moduleType.GetFullName()}]",
                            declaredClusterType.GetFullName()
                            );
                    }

                    var childJoint = new TreeJoint<ITypeSymbol>(
                        joint,
                        declaredClusterType
                        );
                    joint.AddChild(childJoint);

                    BuildDeclaredClusterTree(
                        moduleType,
                        containers,
                        childJoint
                        );
                }
            }
        }
    }

}
