using System;

namespace DpdtInject.Injector.Src.Excp
{
    /// <summary>
    /// An exception type Dpdt are firing in case of something went wrong.
    /// </summary>
    public class DpdtException : Exception
    {
        /// <summary>
        /// Type of failure.
        /// </summary>
        public DpdtExceptionTypeEnum Type
        {
            get;
        }

        /// <summary>
        /// Additional argument associated with this exception.
        /// (usually here is full name of some type).
        /// </summary>
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
