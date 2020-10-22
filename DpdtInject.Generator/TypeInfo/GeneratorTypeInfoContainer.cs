using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DpdtInject.Generator.TypeInfo
{
    public class GeneratorTypeInfoContainer : TypeInfoContainer
    {
        private readonly GeneratorExecutionContext _context;
        private readonly string? _generatedSourceFolderFullPath;

        public int UnitsGenerated
        {
            get;
            private set;
        }

        public GeneratorTypeInfoContainer(
            ref GeneratorExecutionContext context,
            string? generatedSourceFolderFullPath
            ) : base(context.Compilation)
        {
            _context = context;
            _generatedSourceFolderFullPath = generatedSourceFolderFullPath;
            UnitsGenerated = 0;
        }

        public override void AddSources(ModificationDescription[] modificationDescriptions)
        {
            var sourceTexts = new List<SourceText>();
            foreach (var modificationDescription in modificationDescriptions)
            {
                if (_generatedSourceFolderFullPath is not null)
                {
                    modificationDescription.NormalizeWhitespaces();

                    var filePath = Path.Combine(_generatedSourceFolderFullPath, modificationDescription.NewFileName);

                    modificationDescription.SaveToDisk(
                        filePath
                        );
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
