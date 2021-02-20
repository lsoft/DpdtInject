using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Generator.Producer.Product;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Generator.Producer
{
    internal class ClusterProducer
    {
        private readonly ITypeInfoProvider _typeInfoProvider;

        public ClusterBindings ClusterBindings
        {
            get;
        }

        public ClusterProducer(
            ITypeInfoProvider typeInfoProvider,
            ClusterBindings clusterBindings
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            if (clusterBindings is null)
            {
                throw new ArgumentNullException(nameof(clusterBindings));
            }
            _typeInfoProvider = typeInfoProvider;
            ClusterBindings = clusterBindings;
        }


        public string Produce(
            List<UsingDirectiveSyntax> additionalUsings
            )
        {
            if (additionalUsings is null)
            {
                throw new ArgumentNullException(nameof(additionalUsings));
            }

            var instanceProducts = new List<InstanceProduct>();

            foreach (var bindingExtender in ClusterBindings.BindingExtenders)
            {
                IInstanceProducer instanceProducer;
                switch (bindingExtender.BindingContainer.Scope)
                {
                    case BindScopeEnum.Transient:
                        instanceProducer = new TransientInstanceProducer(
                            _typeInfoProvider,
                            ClusterBindings,
                            bindingExtender
                            );
                        break;
                    case BindScopeEnum.Singleton:
                        instanceProducer = new SingletonInstanceProducer(
                            _typeInfoProvider,
                            ClusterBindings,
                            bindingExtender
                            );
                        break;
                    case BindScopeEnum.Constant:
                        instanceProducer = new ConstantInstanceProducer(
                            _typeInfoProvider,
                            ClusterBindings,
                            bindingExtender
                            );
                        break;
                    case BindScopeEnum.Custom:
                        instanceProducer = new CustomInstanceProducer(
                            _typeInfoProvider,
                            ClusterBindings,
                            bindingExtender
                            );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(bindingExtender.BindingContainer.Scope.ToString());
                }

                var instanceProduct = instanceProducer.Produce();
                instanceProducts.Add(instanceProduct);
            }

            var resolutionInterfaceProducts = new List<InstanceResolutionInterfaceProduct>();
            foreach (var bindFrom in ClusterBindings.BindsFrom)
            {
                var resolutionInterfaceProducer = new BindFromResolutionProducer(
                    _typeInfoProvider,
                    ClusterBindings,
                    bindFrom,
                    instanceProducts
                    );

                var resolutionInterfaceProduct = resolutionInterfaceProducer.Produce();
                resolutionInterfaceProducts.Add(resolutionInterfaceProduct);
            }


            var customInstanceContainerSize = ClusterBindings.BindingContainers.Count(bc => bc.Scope == BindScopeEnum.Custom);
            customInstanceContainerSize *= ArgumentWrapperHelper.WrapperCount;
            customInstanceContainerSize *= 2; //factor of 2 for additional sparsity; 
            customInstanceContainerSize += 1; //addition of 1 is because of 0 is not allowed size for container

            var compilationUnit = DpdtInject.Generator.Properties.Resource.CarcassCluster;

            var usings = new HashSet<string>(StringComparer.InvariantCulture);
            foreach (var additionalUsing in additionalUsings)
            {
                usings.Add(additionalUsing.ToString());
            }
            usings.Add($"using {typeof(IResolution).Namespace};");
            usings.Add($"using {typeof(IResolution<object>).Namespace};");
            usings.Add($"using {typeof(IResolutionRequest).Namespace};");
            usings.Add($"using {typeof(IResolutionTarget).Namespace};");

            var fixedCompilationUnit = compilationUnit
                .CheckAndReplace(
                    "//GENERATOR: aggressive inline and optimize",
                    "[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]"
                    )
                .ReplaceLineStartsWith(
                    "namespace",
                    $"namespace {ClusterBindings.ClusterType.ContainingNamespace.ToDisplayString()}"
                    )
                .CheckAndReplace(
                    nameof(CarcassCluster),
                    ClusterBindings.ClusterType.Name
                    )
                .ReplaceLineContains(
                    "//GENERATOR: place for conditional binding count",
                    customInstanceContainerSize.ToString()
                    )
                .CheckAndReplace(
                    "//GENERATOR: place for an additional usings",
                    usings.Join(a => a)
                    )
                .CheckAndReplace(
                    "//GENERATOR: add nongeneric GET binding",
                    (resolutionInterfaceProducts.Count > 0 ? (resolutionInterfaceProducts.Join(rip => rip.ResolutionProducts.Join(rp => rp.NonGenericGetTuple.GetProduct(), ","), ",")) : "")
                    )
                .CheckAndReplace(
                    "//GENERATOR: add nongeneric GET ALL binding",
                    (resolutionInterfaceProducts.Count > 0 ? (resolutionInterfaceProducts.Join(rip => rip.ResolutionProducts.Join(rp => rp.NonGenericGetAllTuple.GetProduct(), ","), ",")) : "")
                    )
                .CheckAndReplace(
                    "//GENERATOR: place for a dispose clauses",
                    instanceProducts.Join(ip => ip.GetDisposeMethodInvokeClause())
                    )
                .CheckAndReplace(
                    "//GENERATOR: place for a resolution interfaces",
                    (resolutionInterfaceProducts.Count > 0 ? ("," + resolutionInterfaceProducts.Join(rip => rip.GetInterfaces(), ",")) : "")
                    )
                .CheckAndReplace(
                    "//GENERATOR: place for an instance interface providers",
                    resolutionInterfaceProducts.Join(rip => rip.GetMethods())
                    )
                .CheckAndReplace(
                    "//GENERATOR: place for an instance object producers",
                    instanceProducts.Join(ip => ip.GetCombinedBody())
                    )
                .CheckAndReplace(
                    "//GENERATOR: place for an unknown type resolutions",
                    instanceProducts.Join(ip => ip.GetCombinedUnknownTypeBody())
                    )
                ;

            return fixedCompilationUnit;
        }
    }
}
