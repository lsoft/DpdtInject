using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;

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

        public Span TypeSpan
        {
            get;
        }

        public CodeLensTarget(
            Guid projectGuid,
            string filePath,
            string fullyQualifiedName,
            Span? typeSpan
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

            if (!typeSpan.HasValue)
            {
                throw new ArgumentNullException(nameof(typeSpan));
            }

            ProjectGuid = projectGuid;
            FilePath = filePath;
            FullyQualifiedName = fullyQualifiedName;
            TypeSpan = typeSpan.Value;
        }
    }
}
