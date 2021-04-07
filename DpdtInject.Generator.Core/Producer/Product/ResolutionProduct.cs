using System;
using DpdtInject.Generator.Core.Producer.Product.Tuple;

namespace DpdtInject.Generator.Core.Producer.Product
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
        public IMethodProduct RetrieveMethod
        {
            get;
        }
        public IMethodProduct RetrieveExplicitMethod
        { 
            get; 
        }
        public CreateTupleProduct NonGenericGetTuple
        { 
            get; 
        }
        public IMethodProduct RetrieveAllMethod 
        {
            get;
        }
        public IMethodProduct RetrieveAllExplicitMethod
        { 
            get; 
        }
        public CreateTupleProduct NonGenericGetAllTuple 
        { 
            get; 
        }
        public IMethodProduct RetrieveFastMethod 
        {
            get;
        }

        public ResolutionProduct(
            IInterfaceProduct interfaceProduct,
            IInterfaceProduct interfaceFastProduct,
            IMethodProduct retrieveMethod,
            IMethodProduct retrieveExplicitMethod,
            CreateTupleProduct nonGenericGetTuple,
            IMethodProduct retrieveAllMethod,
            IMethodProduct retrieveAllExplicitMethod,
            CreateTupleProduct nonGenericGetAllTuple,
            IMethodProduct retrieveFastMethod
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
            RetrieveExplicitMethod.Write(writer, sng);
            writer.WriteLine();
            RetrieveMethod.Write(writer, sng);
            writer.WriteLine();
            RetrieveAllExplicitMethod.Write(writer, sng);
            writer.WriteLine();
            RetrieveAllMethod.Write(writer, sng);
            writer.WriteLine();
            RetrieveFastMethod.Write(writer, sng);
        }

    }

}