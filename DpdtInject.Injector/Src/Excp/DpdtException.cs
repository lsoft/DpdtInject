using System;

namespace DpdtInject.Injector.Src.Excp
{
    public class DpdtException : Exception
    {
        public DpdtExceptionTypeEnum Type
        {
            get;
        }
        public string AdditionalArgument
        {
            get;
        }

        public DpdtException(DpdtExceptionTypeEnum type, string message, string additionalArgument = "")
            : base(message)
        {
            Type = type;
            AdditionalArgument = additionalArgument;
        }

        public DpdtException(DpdtExceptionTypeEnum type, string message, Exception innerException, string additionalArgument = "")
            : base(message, innerException)
        {
            Type = type;
            AdditionalArgument = additionalArgument;
        }


    }
}
