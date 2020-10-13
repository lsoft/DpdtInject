using DpdtInject.Generator;
using DpdtInject.Generator.TypeInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace DpdtInject.Tests
{
    public class PreparatorTypeInfoContainer : TypeInfoContainer
    {

        public PreparatorTypeInfoContainer(
            Compilation compilation
            ) : base(compilation)
        {
        }

        public override void AddSource(ModificationDescription modificationDescription)
        {
            var sourceText = SourceText.From(modificationDescription.NewFileBody, Encoding.UTF8);

            UpdateCompilationWith(sourceText);
        }

        public EmitResult Emit(
            string outputPath
            )
        {
            return _compilation.Emit(outputPath);
        }
    }

}
