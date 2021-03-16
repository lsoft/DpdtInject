using DpdtInject.Injector;
using System;
using System.CodeDom.Compiler;

namespace DpdtInject.Generator.Producer.Product
{
    public class ResolutionProduct
    {
        public IInterfaceProduct InterfaceProduct
        {
            get;
        }
        public IInterfaceProduct InterfaceFastProduct
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
            IInterfaceProduct interfaceProduct,
            IInterfaceProduct interfaceFastProduct,
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


        internal void WriteInterface(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.Write(InterfaceProduct.GetInterfaceDeclaration(sng));
            writer.Write(", ");
            writer.Write(InterfaceFastProduct.GetInterfaceDeclaration(sng));
        }

        internal void WriteMethods(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.WriteLine2(RetrieveExplicitMethod.MethodBody);
            writer.WriteLine();
            writer.WriteLine2(RetrieveMethod.MethodBody);
            writer.WriteLine();
            writer.WriteLine2(RetrieveAllExplicitMethod.MethodBody);
            writer.WriteLine();
            writer.WriteLine2(RetrieveAllMethod.MethodBody);
            writer.WriteLine();
            writer.WriteLine2(RetrieveFastMethod.MethodBody);
        }

    }

}