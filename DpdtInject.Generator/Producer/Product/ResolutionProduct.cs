using DpdtInject.Generator.Producer.Product;
using DpdtInject.Injector.Helper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace DpdtInject.Generator.Producer
{
    public class ResolutionProduct
    {
        public InterfaceProduct InterfaceProduct
        {
            get;
        }
        
        public MethodProduct RetrieveMethod
        {
            get;
        }
        public MethodProduct RetrieveExplicitMethod
        { 
            get; 
        }
        public CreateTupleProduct NonGenericGetTuple
        { 
            get; 
        }
        public MethodProduct RetrieveAllMethod 
        {
            get;
        }
        public MethodProduct RetrieveAllExplicitMethod
        { 
            get; 
        }
        public CreateTupleProduct NonGenericGetAllTuple 
        { 
            get; 
        }

        public ResolutionProduct(
            InterfaceProduct interfaceProduct,
            MethodProduct retrieveMethod,
            MethodProduct retrieveExplicitMethod,
            CreateTupleProduct nonGenericGetTuple,
            MethodProduct retrieveAllMethod,
            MethodProduct retrieveAllExplicitMethod,
            CreateTupleProduct nonGenericGetAllTuple
            )
        {
            if (interfaceProduct is null)
            {
                throw new ArgumentNullException(nameof(interfaceProduct));
            }

            if (retrieveMethod is null)
            {
                throw new ArgumentNullException(nameof(retrieveMethod));
            }

            if (retrieveExplicitMethod is null)
            {
                throw new ArgumentNullException(nameof(retrieveExplicitMethod));
            }

            if (nonGenericGetTuple is null)
            {
                throw new ArgumentNullException(nameof(nonGenericGetTuple));
            }

            if (retrieveAllMethod is null)
            {
                throw new ArgumentNullException(nameof(retrieveAllMethod));
            }

            if (nonGenericGetAllTuple is null)
            {
                throw new ArgumentNullException(nameof(nonGenericGetAllTuple));
            }

            InterfaceProduct = interfaceProduct;
            RetrieveMethod = retrieveMethod;
            RetrieveExplicitMethod = retrieveExplicitMethod;
            NonGenericGetTuple = nonGenericGetTuple;
            RetrieveAllMethod = retrieveAllMethod;
            RetrieveAllExplicitMethod = retrieveAllExplicitMethod;
            NonGenericGetAllTuple = nonGenericGetAllTuple;
        }

        public string GetInterface()
        {
            return $@"
{InterfaceProduct.InterfaceDeclaration}
";
        }

        public string GetMethods()
        {
            return $@"
#region Get

{RetrieveExplicitMethod.MethodBody}

{RetrieveMethod.MethodBody}

#endregion

#region GetAll

{RetrieveAllExplicitMethod.MethodBody}

{RetrieveAllMethod.MethodBody}

#endregion
";
        }
    }

}