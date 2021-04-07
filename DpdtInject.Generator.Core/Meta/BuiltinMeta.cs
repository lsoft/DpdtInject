using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using DpdtInject.Generator.Core.Binding.Xml;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.Meta
{
    public class BuiltinMeta
    {
        private const string SingleLineComment = "//";

        public void Store(
            ITypeInfoContainer typeInfoContainer,
            ProjectBindContainerXml projectXml
            )
        {
            if (typeInfoContainer is null)
            {
                throw new ArgumentNullException(nameof(typeInfoContainer));
            }

            if (projectXml is null)
            {
                throw new ArgumentNullException(nameof(projectXml));
            }

            var sb = new StringBuilder();
            foreach (var line in projectXml.GetXml().Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                sb.Append(SingleLineComment);
                sb.AppendLine(line);
            }

            var xml = sb.ToString();

            typeInfoContainer.AddAdditionalFile(xml);
        }

        public bool TryExtract(
            Compilation compilation,
            [NotNullWhen(true)] out ProjectBindContainerXml? projectXml
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            var xmlFile = compilation.SyntaxTrees.FirstOrDefault(st => st.FilePath == DpdtInternalGenerator.DpdtXmlArtifactFilePath);
            if (xmlFile == null)
            {
                projectXml = null;
                return false;
            }

            var sb = new StringBuilder();
            foreach (var line in xmlFile.GetText().ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                sb.AppendLine(line.Substring(SingleLineComment.Length));
            }

            var xml = sb.ToString();

            projectXml = xml.GetObjectFromXml<ProjectBindContainerXml>();

            return true;
        }
    }
}
