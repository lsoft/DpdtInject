using DpdtInject.Generator.Binding;
using System;

namespace DpdtInject.Generator.Producer
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

        public InstanceProduct(
            BindingContainerExtender bindingExtender,
            MethodProduct? predicateMethod,
            MethodProduct factoryObjectMethod,
            MethodProduct funcMethod,
            MethodProduct? disposeMethod
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

        internal string GetDisposeMethodInvokeClause()
        {
            if(DisposeMethod is null)
            {
                return string.Empty;
            }

            return $"{DisposeMethod.MethodName}();";
        }
    }
}
