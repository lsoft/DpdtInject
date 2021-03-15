using System;
using System.CodeDom.Compiler;

namespace DpdtInject.Generator.Producer.Product
{
    public class ResolutionProduct
    {
        public InterfaceProduct InterfaceProduct
        {
            get;
        }
        public InterfaceProduct InterfaceFastProduct
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
        public MethodProduct RetrieveFastMethod 
        {
            get;
        }

        public ResolutionProduct(
            InterfaceProduct interfaceProduct,
            InterfaceProduct interfaceFastProduct,
            MethodProduct retrieveMethod,
            MethodProduct retrieveExplicitMethod,
            CreateTupleProduct nonGenericGetTuple,
            MethodProduct retrieveAllMethod,
            MethodProduct retrieveAllExplicitMethod,
            CreateTupleProduct nonGenericGetAllTuple,
            MethodProduct retrieveFastMethod
            )
        {
            if (interfaceProduct is null)
            {
                throw new ArgumentNullException(nameof(interfaceProduct));
            }

            if (interfaceFastProduct is null)
            {
                throw new ArgumentNullException(nameof(interfaceFastProduct));
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

            if (retrieveFastMethod is null)
            {
                throw new ArgumentNullException(nameof(retrieveFastMethod));
            }

            InterfaceProduct = interfaceProduct;
            InterfaceFastProduct = interfaceFastProduct;
            RetrieveMethod = retrieveMethod;
            RetrieveExplicitMethod = retrieveExplicitMethod;
            NonGenericGetTuple = nonGenericGetTuple;
            RetrieveAllMethod = retrieveAllMethod;
            RetrieveAllExplicitMethod = retrieveAllExplicitMethod;
            NonGenericGetAllTuple = nonGenericGetAllTuple;
            RetrieveFastMethod = retrieveFastMethod;
        }


        internal void WriteInterface(IndentedTextWriter writer)
        {
            writer.Write(InterfaceProduct.InterfaceDeclaration);
            writer.Write(", ");
            writer.Write(InterfaceFastProduct.InterfaceDeclaration);
        }

        internal void WriteMethods(IndentedTextWriter writer)
        {
            writer.WriteLine(RetrieveExplicitMethod.MethodBody);
            writer.WriteLine();
            writer.WriteLine(RetrieveMethod.MethodBody);
            writer.WriteLine();
            writer.WriteLine(RetrieveAllExplicitMethod.MethodBody);
            writer.WriteLine();
            writer.WriteLine(RetrieveAllMethod.MethodBody);
            writer.WriteLine();
            writer.WriteLine(RetrieveFastMethod.MethodBody);
        }


//        public string GetMethods()
//        {
//            return $@"
//#region Get

//{RetrieveExplicitMethod.MethodBody}

//{RetrieveMethod.MethodBody}

//#endregion

//#region GetAll

//{RetrieveAllExplicitMethod.MethodBody}

//{RetrieveAllMethod.MethodBody}

//#endregion

//#region GetFast

//{RetrieveFastMethod.MethodBody}

//#endregion
//";
//        }

    }

}