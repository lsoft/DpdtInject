using DpdtInject.Generator.Helpers;
using DpdtInject.Injector;
using DpdtInject.Injector.Beautify;
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
        public string ClusterTypeFullName
        {
            get;
        }
        
        public string ClassName
        {
            get;
        }


        public BeautifyGenerator(
            string clusterTypeFullName
            )
        {
            if (clusterTypeFullName is null)
            {
                throw new ArgumentNullException(nameof(clusterTypeFullName));
            }

            ClusterTypeFullName = clusterTypeFullName;

            ClassName = $"{nameof(Beautifier)}_{Guid.NewGuid().RemoveMinuses()}";

        }

        public string GenerateBeautifierBody(
            )
        {
            var bus = SyntaxFactory.ParseCompilationUnit(Properties.Resources.Beautifier);
            var bds = bus.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

            var classBody = bds.GetText().ToString()
                .CheckAndReplace(nameof(IBeautifier), typeof(IBeautifier).FullName!)
                .CheckAndReplace(nameof(IListBeautifier), typeof(IListBeautifier).FullName!)
                .CheckAndReplace(nameof(IReadOnlyListBeautifier), typeof(IReadOnlyListBeautifier).FullName!)
                .CheckAndReplace($" {nameof(Beautifier)}", $" {ClassName}")
                .CheckAndReplace(nameof(FakeCluster), ClusterTypeFullName)
                //.CheckAndReplace("public sealed class", "private sealed class")
                ;

            return classBody;
        }
    }
}
