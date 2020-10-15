using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Product;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Producer.Factory
{
    internal class FactoryProducer
    {
        private readonly ITypeInfoProvider _typeInfoProvider;
        
        public ClusterBindings ClusterBindings
        {
            get;
        }

        public FactoryProducer(
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

        internal IEnumerable<FactoryProduct> Produce()
        {
            foreach(var bindingContainer in ClusterBindings.BindingContainers)
            {
                if (!bindingContainer.ToFactory)
                {
                    continue;
                }

                var factoryProduct = GenerateFactoryBody(bindingContainer);
                yield return factoryProduct;
            }
        }

        private FactoryProduct GenerateFactoryBody(
            IBindingContainer bindingContainer
            )
        {
            var methodsToImplement = ScanForMethodsToImplement(bindingContainer);

            return new FactoryProduct(
                bindingContainer.BindToType,
                $@"
namespace DpdtInject.Tests.Factory.Simple0
{{
    public partial class AFactory : IAFactory
    {{
        public IA Create()
        {{
            return new A();
        }}
    }}
}}
");
        }

        private List<IMethodSymbol> ScanForMethodsToImplement(
            IBindingContainer bindingContainer
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            var toImplementedMethods = new List<IMethodSymbol>();
            var declaredMethods = bindingContainer.BindFromTypes[0].GetMembers().FindAll(m => m.Kind == SymbolKind.Method);

            foreach (IMethodSymbol declaredMethod in declaredMethods)
            {
                var implementedeMethod = bindingContainer.BindToType.FindImplementationForInterfaceMember(declaredMethod);
                if(!(implementedeMethod is null))
                {
                    continue;
                }

                //method is not implemented in the proto class
                if(!bindingContainer.FactoryPayloadType!.CanBeCastedTo(declaredMethod.ReturnType.ToDisplayString()))
                {
                    //return type is not a factory payload
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectReturnType,
                        $"Factory [{bindingContainer.BindToType.ToDisplayString()}] contains a method [{declaredMethod.Name}] with return type [{declaredMethod.ReturnType.ToDisplayString()}] that cannot be produced by casting a payload [{bindingContainer.FactoryPayloadType!.ToDisplayString()}]",
                        bindingContainer.BindToType.ToDisplayString()
                        );
                }

                toImplementedMethods.Add(declaredMethod);
            }

            return toImplementedMethods;
        }
    }
}
