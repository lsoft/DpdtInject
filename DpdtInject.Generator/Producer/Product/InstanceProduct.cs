using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Binding;
using DpdtInject.Injector;

namespace DpdtInject.Generator.Producer.Product
{
    public class InstanceProduct
    {
        public BindingContainerExtender BindingExtender
        {
            get;
        }
        
        public MethodProduct? PredicateMethod 
        {
            get;
        }

        public MethodProduct FactoryObjectMethod 
        { 
            get; 
        }
        public MethodProduct FuncMethod
        {
            get;
        }
        public MethodProduct? DisposeMethod
        {
            get;
        }

        public IReadOnlyList<UnknownTypeProduct> UnknownTypeProducts
        {
            get;
        }

        public InstanceProduct(
            BindingContainerExtender bindingExtender,
            MethodProduct? predicateMethod,
            MethodProduct factoryObjectMethod,
            MethodProduct funcMethod,
            MethodProduct? disposeMethod,
            IReadOnlyList<UnknownTypeProduct>? unknownTypeProducts
            )
        {
            if (bindingExtender is null)
            {
                throw new ArgumentNullException(nameof(bindingExtender));
            }

            if (factoryObjectMethod is null)
            {
                throw new ArgumentNullException(nameof(factoryObjectMethod));
            }

            if (funcMethod is null)
            {
                throw new ArgumentNullException(nameof(funcMethod));
            }

            BindingExtender = bindingExtender;
            PredicateMethod = predicateMethod;
            FactoryObjectMethod = factoryObjectMethod;
            FuncMethod = funcMethod;
            DisposeMethod = disposeMethod;
            UnknownTypeProducts = unknownTypeProducts ?? new List<UnknownTypeProduct>();
        }

        internal void WriteCombinedBody(IndentedTextWriter2 writer)
        {
            if (PredicateMethod != null)
            {
                writer.WriteLine2(PredicateMethod.MethodBody);
            }

            writer.WriteLine2(FactoryObjectMethod.MethodBody);
            writer.WriteLine2(FuncMethod.MethodBody);

            if (DisposeMethod != null)
            {
                writer.WriteLine2(DisposeMethod.MethodBody);
            }
        }

        internal void WriteDisposeMethodInvoke(IndentedTextWriter2 writer)
        {
            if (DisposeMethod is null)
            {
                return;
            }

            writer.WriteLine($"TryToSafeDispose({DisposeMethod.MethodName}, ref result);");
        }


        public void WriteCombinedUnknownTypeBody(IndentedTextWriter2 writer)
        {
            if (UnknownTypeProducts.Count == 0)
            {
                return;
            }

            foreach (var utp in UnknownTypeProducts)
            {
                utp.WriteBody(writer);
            }
        }
    }
}
