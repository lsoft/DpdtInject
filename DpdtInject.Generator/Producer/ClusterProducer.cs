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
        private readonly bool _doBeautify;

        public ClusterBindings ClusterBindings
        {
            get;
        }

        public ClusterProducer(
            ITypeInfoProvider typeInfoProvider,
            ClusterBindings clusterBindings,
            bool doBeautify
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
            _doBeautify = doBeautify;
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

            var sng = new ShortTypeNameGenerator();


            var itwMethods = new IndentedTextWriter2(2, _doBeautify);
            var itwInterfaces = new IndentedTextWriter2(3, _doBeautify);
            var itwNonGenericInterfaces = new IndentedTextWriter2(4, _doBeautify);
            var itwNonGenericGetAllInterfaces = new IndentedTextWriter2(4, _doBeautify);

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
                    itwNonGenericGetAllInterfaces,
                    sng
                    );
            }


            var itwDispose = new IndentedTextWriter2(3, _doBeautify);
            var itwCombinedBody = new IndentedTextWriter2(4, _doBeautify);
            var itwCombinedUnknownTypeBody = new IndentedTextWriter2(4, _doBeautify);

            foreach (var instanceProduct in instanceProducts)
            {
                instanceProduct.WriteDisposeMethodInvoke(itwDispose, sng);
                instanceProduct.WriteCombinedBody(itwCombinedBody, sng);
                instanceProduct.WriteCombinedUnknownTypeBody(itwCombinedUnknownTypeBody, sng);
            }

            sng.WriteUsings(usings);

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
                    itwNonGenericInterfaces.GetResultString()
                    )
                .Replace(
                    "//GENERATOR: add nongeneric GET ALL binding",
                    itwNonGenericGetAllInterfaces.GetResultString()
                    )
                .Replace(
                    "//GENERATOR: place for a dispose clauses",
                    itwDispose.GetResultString()
                    )
                .Replace(
                    "//GENERATOR: place for a resolution interfaces",
                    itwInterfaces.GetResultString()
                    )
                .Replace(
                    "//GENERATOR: place for an instance interface providers",
                    itwMethods.GetResultString()
                    )
                .Replace(
                    "//GENERATOR: place for an instance object producers",
                    itwCombinedBody.GetResultString()
                    )
                .Replace(
                    "//GENERATOR: place for an unknown type resolutions",
                    itwCombinedUnknownTypeBody.GetResultString()
                    )
                .ToString();

            return fixedCompilationUnit;
        }
    }

    public class ShortTypeNameGenerator
    {
        private readonly Dictionary<string, string> _nameDict = new Dictionary<string, string>();
        public ShortTypeNameGenerator()
        {

        }

        public string GetShortName(ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var key = type.ToDisplayString();
            if (_nameDict.TryGetValue(key, out var shortName))
            {
                return shortName;
            }

            while (true)
            {
                var newShortName = type.GetSpecialName() + Guid.NewGuid().RemoveMinuses().Substring(0, 8);
                if (_nameDict.ContainsKey(newShortName))
                {
                    continue;
                }

                _nameDict[key] = newShortName;

                return newShortName;
            }
        }


        public void WriteUsings(
            HashSet<string> set
            )
        {
            //using q123 = System.Func<System.String>;
            foreach (var pair in _nameDict)
            {
                set.Add($"using {pair.Value} = {pair.Key};");
            }
        }

    }
}
