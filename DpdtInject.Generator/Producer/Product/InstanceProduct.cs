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
        
        public IMethodProduct? PredicateMethod 
        {
            get;
        }

        public IMethodProduct FactoryObjectMethod 
        { 
            get; 
        }
        public IMethodProduct FuncMethod
        {
            get;
        }
        public IMethodProduct? DisposeMethod
        {
            get;
        }

        public IReadOnlyList<UnknownTypeProduct> UnknownTypeProducts
        {
            get;
        }

        public InstanceProduct(
            BindingContainerExtender bindingExtender,
            IMethodProduct? predicateMethod,
            IMethodProduct factoryObjectMethod,
            IMethodProduct funcMethod,
            IMethodProduct? disposeMethod,
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

        internal void WriteCombinedBody(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            if (PredicateMethod != null)
            {
                PredicateMethod.Write(writer, sng);
            }

            FactoryObjectMethod.Write(writer, sng);
            FuncMethod.Write(writer, sng);

            if (DisposeMethod != null)
            {
                DisposeMethod.Write(writer, sng);
            }
        }

        internal void WriteDisposeMethodInvoke(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            if (DisposeMethod is null)
            {
                return;
            }

            writer.WriteLine($"TryToSafeDispose({DisposeMethod.MethodName}, ref result);");
        }


        public void WriteCombinedUnknownTypeBody(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            if (UnknownTypeProducts.Count == 0)
            {
                return;
            }

            foreach (var utp in UnknownTypeProducts)
            {
                utp.WriteBody(writer, sng);
            }
        }
    }
}
