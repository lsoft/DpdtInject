using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Binding;

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

        internal string GetCombinedBody()
        {
            return
                (PredicateMethod is null ? "" : PredicateMethod.MethodBody)
                + Environment.NewLine + FactoryObjectMethod.MethodBody
                + Environment.NewLine + FuncMethod.MethodBody
                + Environment.NewLine + (DisposeMethod is null ? "" : DisposeMethod.MethodBody)
                ;
        }

        internal void WriteCombinedBody(IndentedTextWriter writer)
        {
            if (PredicateMethod != null)
            {
                writer.WriteLine(PredicateMethod.MethodBody);
            }

            writer.WriteLine(FactoryObjectMethod.MethodBody);
            writer.WriteLine(FuncMethod.MethodBody);

            if (DisposeMethod != null)
            {
                writer.WriteLine(DisposeMethod.MethodBody);
            }
        }

        internal void WriteDisposeMethodInvoke(IndentedTextWriter writer)
        {
            if (DisposeMethod is null)
            {
                return;
            }

            writer.WriteLine($"{DisposeMethod.MethodName}();");
        }


        public void WriteCombinedUnknownTypeBody(IndentedTextWriter writer)
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

        //public string GetCombinedUnknownTypeBody()
        //{
        //    if (UnknownTypeProducts.Count == 0)
        //    {
        //        return string.Empty;
        //    }

        //    return string.Join(
        //        Environment.NewLine,
        //        UnknownTypeProducts.Select(utp => utp.GetBody())
        //        );
        //}
    }
}
