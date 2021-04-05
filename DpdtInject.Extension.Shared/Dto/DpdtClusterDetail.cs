using System;

namespace DpdtInject.Extension.Shared.Dto
{
    public class DpdtClusterDetail : IDpdtClusterDetail
    {
        public string ClassNamespace
        {
            get;
            set;
        }

        public string ClassFullName
        {
            get;
            set;
        }

        public string MethodName
        {
            get;
            set;
        }


        public string FullName => $"{ClassFullName}.{MethodName}";

        public DpdtClusterDetail(
            string classNamespace,
            string classFullName,
            string methodName
            )
        {
            if (classNamespace is null)
            {
                throw new ArgumentNullException(nameof(classNamespace));
            }

            if (classFullName is null)
            {
                throw new ArgumentNullException(nameof(classFullName));
            }

            if (methodName is null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }
            ClassNamespace = classNamespace;
            ClassFullName = classFullName;
            MethodName = methodName;
        }
    }
}