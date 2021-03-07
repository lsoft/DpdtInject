using System;

namespace DpdtInject.Extension.Shared.Dto
{
    public class CodeLensTarget
    {
        public Guid ProjectGuid
        {
            get;
            set;
        }

        public string FilePath
        {
            get;
            set;
        }

        public string FullyQualifiedName
        {
            get;
            set;
        }
        public int TypeSpanStart
        {
            get;
            set;
        }
        public int TypeSpanLength
        {
            get;
            set;
        }

        public CodeLensTarget(
            Guid projectGuid,
            string filePath,
            string fullyQualifiedName,
            int? typeSpanStart,
            int? typeSpanLength
            )
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (fullyQualifiedName is null)
            {
                throw new ArgumentNullException(nameof(fullyQualifiedName));
            }

            if (!typeSpanStart.HasValue)
            {
                throw new ArgumentNullException(nameof(typeSpanStart));
            }
            if (!typeSpanLength.HasValue)
            {
                throw new ArgumentNullException(nameof(typeSpanLength));
            }

            ProjectGuid = projectGuid;
            FilePath = filePath;
            FullyQualifiedName = fullyQualifiedName;
            TypeSpanStart = typeSpanStart.Value;
            TypeSpanLength = typeSpanLength.Value;
        }
    }
}
