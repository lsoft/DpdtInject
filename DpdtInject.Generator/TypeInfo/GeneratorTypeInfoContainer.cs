using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DpdtInject.Generator.TypeInfo
{
    public class GeneratorTypeInfoContainer : TypeInfoContainer
    {
        private readonly GeneratorExecutionContext _context;

        public int UnitsGenerated
        {
            get;
            private set;
        }

        public GeneratorTypeInfoContainer(
            ref GeneratorExecutionContext context
            ) : base(context.Compilation)
        {
            _context = context;
            UnitsGenerated = 0;
        }

        public override void AddSources(ModificationDescription[] modificationDescriptions)
        {
            var sourceTexts = new List<SourceText>();
            foreach (var modificationDescription in modificationDescriptions)
            {
                var sourceText = SourceText.From(modificationDescription.NewFileBody, Encoding.UTF8);

                _context.AddSource(
                    modificationDescription.NewFileName,
                    sourceText
                    );

                sourceTexts.Add(sourceText);

                UnitsGenerated++;
            }

            UpdateCompilationWith(sourceTexts.ToArray());
        }


    }
}
