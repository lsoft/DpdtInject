using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Producer.Product;
using DpdtInject.Injector.Bind;
using DpdtInject.Injector.RContext;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

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
            IReadOnlyList<UsingDirectiveSyntax> additionalUsings
            )
        {
            if (additionalUsings is null)
            {
                throw new ArgumentNullException(nameof(additionalUsings));
            }

            var instanceProducts = new List<InstanceProduct>();
            var instanceProductDict = new Dictionary<ITypeSymbol, List<InstanceProduct>>(SymbolEqualityComparer.Default);

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

                foreach (var bindFrom in instanceProduct.BindingExtender.BindingContainer.BindFromTypes)
                {
                    if (!instanceProductDict.ContainsKey(bindFrom))
                    {
                        instanceProductDict[bindFrom] = new List<InstanceProduct>();
                    }

                    instanceProductDict[bindFrom].Add(instanceProduct);
                }
            }

            var resolutionInterfaceProducts = new List<InstanceResolutionInterfaceProduct>();
            foreach (var pair in instanceProductDict)
            {
                var bindFrom = pair.Key;
                var filteredInstanceProducts = pair.Value;

                var resolutionInterfaceProducer = new BindFromResolutionProducer(
                    _typeInfoProvider,
                    ClusterBindings,
                    bindFrom,
                    filteredInstanceProducts
                    );

                var resolutionInterfaceProduct = resolutionInterfaceProducer.Produce();
                resolutionInterfaceProducts.Add(resolutionInterfaceProduct);
            }


            var customInstanceContainerSize = ClusterBindings.BindingContainers.Count(bc => bc.Scope == BindScopeEnum.Custom);
            customInstanceContainerSize *= ArgumentWrapperHelper.WrapperCountIncludeNone;
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



            var sbMethods = new StringBuilder();
            var itwMethods = new IndentedTextWriter(new StringWriter(sbMethods), IndentedTextWriter.DefaultTabString);
            itwMethods.Indent = 2;

            var sbInterfaces = new StringBuilder();
            var itwInterfaces = new IndentedTextWriter(new StringWriter(sbInterfaces), IndentedTextWriter.DefaultTabString);
            itwInterfaces.Indent = 3;

            var sbNonGenericInterfaces = new StringBuilder();
            var itwNonGenericInterfaces = new IndentedTextWriter(new StringWriter(sbNonGenericInterfaces), IndentedTextWriter.DefaultTabString);
            itwNonGenericInterfaces.Indent = 4;

            var sbNonGenericGetAllInterfaces = new StringBuilder();
            var itwNonGenericGetAllInterfaces = new IndentedTextWriter(new StringWriter(sbNonGenericGetAllInterfaces), IndentedTextWriter.DefaultTabString);
            itwNonGenericGetAllInterfaces.Indent = 4;

            if (resolutionInterfaceProducts.Count > 0)
            {
                itwInterfaces.WriteLine(",");
            }

            foreach (var (resolutionInterfaceProduct, isLast) in resolutionInterfaceProducts.IterateWithLastSignal())
            {
                resolutionInterfaceProduct.Write(
                    isLast,
                    itwMethods,
                    itwInterfaces,
                    itwNonGenericInterfaces,
                    itwNonGenericGetAllInterfaces
                    );
            }

            itwMethods.Flush();
            itwInterfaces.Flush();
            itwNonGenericInterfaces.Flush();
            itwNonGenericGetAllInterfaces.Flush();


            var sbDispose = new StringBuilder();
            var itwDispose = new IndentedTextWriter(new StringWriter(sbDispose), IndentedTextWriter.DefaultTabString);
            itwDispose.Indent = 4;

            var sbCombinedBody = new StringBuilder();
            var itwCombinedBody = new IndentedTextWriter(new StringWriter(sbCombinedBody), IndentedTextWriter.DefaultTabString);
            itwCombinedBody.Indent = 4;

            var sbCombinedUnknownTypeBody = new StringBuilder();
            var itwCombinedUnknownTypeBody = new IndentedTextWriter(new StringWriter(sbCombinedUnknownTypeBody), IndentedTextWriter.DefaultTabString);
            itwCombinedUnknownTypeBody.Indent = 4;

            foreach (var instanceProduct in instanceProducts)
            {
                instanceProduct.WriteDisposeMethodInvoke(itwDispose);
                instanceProduct.WriteCombinedBody(itwCombinedBody);
                instanceProduct.WriteCombinedUnknownTypeBody(itwCombinedUnknownTypeBody);
            }

            itwDispose.Flush();
            itwCombinedBody.Flush();
            itwCombinedUnknownTypeBody.Flush();

            var sbCompilationUnit = new StringBuilder(compilationUnit);

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
                .AsStringBuilder()
                .Replace(
                    "//GENERATOR: add nongeneric GET binding",
                    sbNonGenericInterfaces.ToString()
                    )
                .Replace(
                    "//GENERATOR: add nongeneric GET ALL binding",
                    sbNonGenericGetAllInterfaces.ToString()
                    )
                .Replace(
                    "//GENERATOR: place for a dispose clauses",
                    sbDispose.ToString()
                    )
                .Replace(
                    "//GENERATOR: place for a resolution interfaces",
                    sbInterfaces.ToString()
                    )
                .Replace(
                    "//GENERATOR: place for an instance interface providers",
                    sbMethods.ToString()
                    )
                .Replace(
                    "//GENERATOR: place for an instance object producers",
                    sbCombinedBody.ToString()
                    )
                .Replace(
                    "//GENERATOR: place for an unknown type resolutions",
                    sbCombinedUnknownTypeBody.ToString()
                    )
                .ToString();

            return fixedCompilationUnit;
        }
    }
}
