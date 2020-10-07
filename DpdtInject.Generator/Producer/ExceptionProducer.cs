using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Producer
{
    internal class ExceptionProducer
    {
        public ExceptionProducer()
        {

        }

        public static string GenerateThrowExceptionClause(
            DpdtExceptionTypeEnum exceptionType,
            string message
            )
        {
            return
                $"throw new {typeof(DpdtException).FullName}({nameof(DpdtExceptionTypeEnum)}.{exceptionType}, \"{message}\");";
        }

        public static string GenerateThrowExceptionClause(
            DpdtExceptionTypeEnum exceptionType,
            string message,
            string additionalInformation
            )
        {
            return
                $"throw new {typeof(DpdtException).FullName}({nameof(DpdtExceptionTypeEnum)}.{exceptionType}, \"{message}\", \"{additionalInformation}\");";
        }

        public static string GenerateThrowExceptionClause2(
            DpdtExceptionTypeEnum exceptionType,
            string message,
            string additionalInformation
            )
        {
            return
                $"throw new {typeof(DpdtException).FullName}({nameof(DpdtExceptionTypeEnum)}.{exceptionType}, {message}, {additionalInformation});";
        }
    }
}
