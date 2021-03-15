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
        private readonly bool _doBeautify;

        public int UnitsGenerated
        {
            get;
            private set;
        }

        public GeneratorTypeInfoContainer(
            ref GeneratorExecutionContext context,
            bool doBeautify
            ) : base(context.Compilation)
        {
            _context = context;
            _doBeautify = doBeautify;
            UnitsGenerated = 0;
        }

        public override void AddSources(ModificationDescription[] modificationDescriptions)
        {
            var sourceTexts = new List<SourceText>();
            foreach (var modificationDescription in modificationDescriptions)
            {
                if (_doBeautify)
                {
                    modificationDescription.NormalizeWhitespaces();
                }

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
