using DpdtInject.Generator.TypeInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.IO;
using System.Text;

namespace DpdtInject.Generator
{
    public class GeneratorTypeInfoContainer : TypeInfoContainer
    {
        private readonly GeneratorExecutionContext _context;
        private readonly bool _needToStoreGeneratedSources;
        private readonly string _generatedSourceFolderFullPath;

        public int UnitsGenerated
        {
            get;
            private set;
        }

        public GeneratorTypeInfoContainer(
            ref GeneratorExecutionContext context,
            bool needToStoreGeneratedSources,
            string generatedSourceFolderFullPath
            ) : base(context.Compilation)
        {
            _context = context;
            _needToStoreGeneratedSources = needToStoreGeneratedSources;
            _generatedSourceFolderFullPath = generatedSourceFolderFullPath;
            UnitsGenerated = 0;
        }

        public override void AddSource(ModificationDescription modificationDescription)
        {
            if (_needToStoreGeneratedSources)
            {
                File.WriteAllText(
                    Path.Combine(_generatedSourceFolderFullPath, modificationDescription.NewFileName),
                    modificationDescription.NewFileBody
                    );
            }

            var sourceText = SourceText.From(modificationDescription.NewFileBody, Encoding.UTF8);

            _context.AddSource(
                modificationDescription.NewFileName,
                sourceText
                );


            UpdateCompilationWith(sourceText);


            UnitsGenerated++;
        }
    }
}
