using System;
using System.Collections.Generic;
using DpdtInject.Generator.Core.Binding;

namespace DpdtInject.Generator.Core.Producer.Product
{
    public class InstanceProduct
    {
        public BindingExtender BindingExtender
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

        public IMethodProduct? AsyncDisposeMethod
        {
            get;
        }

        public IReadOnlyList<UnknownTypeProduct> UnknownTypeProducts
        {
            get;
        }

        public InstanceProduct(
            BindingExtender bindingExtender,
            IMethodProduct? predicateMethod,
            IMethodProduct factoryObjectMethod,
            IMethodProduct funcMethod,
            IMethodProduct? disposeMethod,
            IMethodProduct? asyncDisposeMethod,
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
            AsyncDisposeMethod = asyncDisposeMethod;
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
            if (AsyncDisposeMethod != null)
            {
                AsyncDisposeMethod.Write(writer, sng);
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

        internal void WriteAsyncDisposeMethodInvoke(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            if (AsyncDisposeMethod is null)
            {
                return;
            }

            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine($"var excp = await TryToSafeDisposeAsync({AsyncDisposeMethod.MethodName}).ConfigureAwait(false);");
            writer.WriteLine($"if(excp != null)");
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine($"result.Add(excp);");
            writer.Indent--;
            writer.WriteLine("}");
            writer.Indent--;
            writer.WriteLine("}");
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
