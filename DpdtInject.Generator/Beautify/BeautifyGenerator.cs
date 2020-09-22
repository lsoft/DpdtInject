using DpdtInject.Generator.Helpers;
using DpdtInject.Injector;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Beautify
{
    public sealed class BeautifyGenerator
    {
        public string ModuleFullTypeName { get; }
        public string ClassName { get; }


        public BeautifyGenerator(string moduleFullTypeName)
        {
            if (moduleFullTypeName is null)
            {
                throw new ArgumentNullException(nameof(moduleFullTypeName));
            }

            ModuleFullTypeName = moduleFullTypeName;

            ClassName = $"{nameof(Beautifier)}_{Guid.NewGuid().RemoveMinuses()}";

        }

        public string GenerateBeautifierBody(
            )
        {
            var bus = SyntaxFactory.ParseCompilationUnit(Properties.Resources.Beautifier);
            var bds = bus.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

            var classBody = bds.GetText().ToString()
                .CheckAndReplace(nameof(Beautifier), ClassName)
                .CheckAndReplace(nameof(FakeModule), ModuleFullTypeName)
                //.CheckAndReplace("public sealed class", "private sealed class")
                ;

            return classBody;
        }
    }
}
