using System;
using System.Collections.Generic;
using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core
{
    public class ClusterMethodBindings
    {
        private readonly List<(MethodDeclarationSyntax, IReadOnlyList<IBindingContainer>)> _methodBindings = new();

        public INamedTypeSymbol ClusterType
        {
            get;
        }

        public IReadOnlyList<(MethodDeclarationSyntax, IReadOnlyList<IBindingContainer>)> MethodBindings => _methodBindings;

        public IReadOnlyList<UsingDirectiveSyntax> ModuleUnitUsings
        {
            get;
        }

        public ClusterMethodBindings(
            INamedTypeSymbol clusterType,
            List<UsingDirectiveSyntax> moduleUnitUsings
            )
        {
            if (clusterType is null)
            {
                throw new ArgumentNullException(nameof(clusterType));
            }
            if (moduleUnitUsings is null)
            {
                throw new ArgumentNullException(nameof(moduleUnitUsings));
            }

            ClusterType = clusterType;
            ModuleUnitUsings = moduleUnitUsings;
        }

        public ClusterBindings GetClusterBindings()
        {
            var allBindingContainers = new List<IBindingContainer>();

            foreach (var methodBinding in _methodBindings)
            {
                allBindingContainers.AddRange(methodBinding.Item2);
            }

            return
                new(
                    ClusterType,
                    allBindingContainers
                    );
        }

        public void AddMethodBindings(
            MethodDeclarationSyntax bindMethodSyntax,
            IReadOnlyList<IBindingContainer> bindingContainers
            )
        {
            if (bindMethodSyntax is null)
            {
                throw new ArgumentNullException(nameof(bindMethodSyntax));
            }

            if (bindingContainers is null)
            {
                throw new ArgumentNullException(nameof(bindingContainers));
            }

            _methodBindings.Add(
                (bindMethodSyntax, bindingContainers)
                );
        }
    }
}
