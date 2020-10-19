using DpdtInject.Generator;
using DpdtInject.Generator.TypeInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
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

        public override void AddSources(ModificationDescription[] modificationDescriptions)
        {
            var sourceTexts = new List<SourceText>();
            foreach (var modificationDescription in modificationDescriptions)
            {
                var sourceText = SourceText.From(modificationDescription.NewFileBody, Encoding.UTF8);

                sourceTexts.Add(sourceText);
            }

            UpdateCompilationWith(sourceTexts.ToArray());
        }

        public EmitResult Emit(
            string outputPath
            )
        {
            return _compilation.Emit(outputPath);
        }
    }

}
