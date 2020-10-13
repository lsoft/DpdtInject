using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public string Produce()
        {
            var compilationUnit = DpdtInject.Generator.Properties.Resource.CarcassCluster;

            var instanceProducts = new List<InstanceProduct>();

            IInstanceProducer instanceProducer;
            foreach (var bindingExtender in ClusterBindings.BindingExtenders)
            {
                switch (bindingExtender.BindingContainer.Scope)
                {
                    case Injector.Module.Bind.BindScopeEnum.Transient:
                        instanceProducer = new TransientInstanceProducer(
                            _typeInfoProvider,
                            ClusterBindings,
                            bindingExtender
                            );
                        break;
                    case Injector.Module.Bind.BindScopeEnum.Singleton:
                        instanceProducer = new SingletonInstanceProducer(
                            _typeInfoProvider,
                            ClusterBindings,
                            bindingExtender
                            );
                        break;
                    case Injector.Module.Bind.BindScopeEnum.Constant:
                        instanceProducer = new ConstantInstanceProducer(
                            _typeInfoProvider,
                            ClusterBindings,
                            bindingExtender
                            );
                        break;
                    case Injector.Module.Bind.BindScopeEnum.Custom:
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

            var fixedCompilationUnit = compilationUnit
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
                    new[]
                    {
                    $"using {typeof(IResolution).Namespace};",
                    $"using {typeof(IResolution<object>).Namespace};",
                    $"using {typeof(IResolutionRequest).Namespace};",
                    $"using {typeof(IResolutionTarget).Namespace};",
                    //$"using {typeof(ResolutionContext<object, object, object>).Namespace};",
                    //$"using {typeof(IResolutionContext).Namespace};",
                    }.Join(a => a)
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
                    "//GENERATOR: place for an intance interface providers",
                    resolutionInterfaceProducts.Join(rip => rip.GetMethods())
                    )
                .CheckAndReplace(
                    "//GENERATOR: place for an intance object producers",
                    instanceProducts.Join(ip => ip.GetCombinedBody())
                    )
                ;

            return fixedCompilationUnit;
        }
    }
}
