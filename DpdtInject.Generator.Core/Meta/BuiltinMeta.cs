using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Generator.Core.Binding.Xml;
using DpdtInject.Generator.Core.TypeInfo;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core.Meta
{
    public class BuiltinMeta
    {

        public void Store(
            ITypeInfoContainer typeInfoContainer,
            SolutionBindContainerXml solutionXml
            )
        {
            if (typeInfoContainer is null)
            {
                throw new ArgumentNullException(nameof(typeInfoContainer));
            }

            if (solutionXml is null)
            {
                throw new ArgumentNullException(nameof(solutionXml));
            }

            var xml = @$"
namespace Dpdt.Xml
{{
    private class DpdtXml
    {{
        private const string DpdtXmlBody = @""{solutionXml.GetXml().Replace("\"", "\"\"")}"";
    }}
}}
";
            typeInfoContainer.AddAdditionalFile(xml);
        }

        public bool TryExtract(
            Compilation compilation,
            out SolutionBindContainerXml? solutionXml)
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            var xmlType = compilation.GetTypeByMetadataName("Dpdt.Xml.DpdtXml");
            if (xmlType == null)
            {
                solutionXml = null;
                return false;
            }

            if (xmlType.Locations.Length != 1)
            {
                solutionXml = null;
                return false;
            }

            var source = xmlType.Locations[0].SourceTree!;

            var semanticModel = compilation.GetSemanticModel(
                source,
                true
                );
            if (semanticModel == null)
            {
                solutionXml = null;
                return false;
            }

            var constant = source.GetRoot()
                    .DescendantNodes()
                    .OfType<LiteralExpressionSyntax>()
                    .FirstOrDefault()
                ;
            if (constant == null)
            {
                solutionXml = null;
                return false;
            }

            var cv = semanticModel.GetConstantValue(constant);
            var cvs = cv.Value as string;
            solutionXml = cvs!.GetObjectFromXml<SolutionBindContainerXml>();
            return true;
        }
    }
}
