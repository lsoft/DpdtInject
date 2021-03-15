using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;

namespace DpdtInject.Generator
{
    public class ModificationDescription
    {
        public INamedTypeSymbol ModifiedType { get; }

        public string ModifiedTypeName => ModifiedType.Name;

        public string ModifiedTypeFullName => ModifiedType.ToDisplayString();

        public string NewFileName { get; }

        public string NewFileBody
        {
            get;
            private set;
        }

        public ModificationDescription(
            INamedTypeSymbol modifiedType,
            string newFileName,
            string newFileBody
            )
        {
            if (modifiedType is null)
            {
                throw new ArgumentNullException(nameof(modifiedType));
            }

            if (newFileName is null)
            {
                throw new ArgumentNullException(nameof(newFileName));
            }

            ModifiedType = modifiedType;
            NewFileName = newFileName;
            NewFileBody = newFileBody;
        }


        public void SaveToDisk(
            string generatedFilePath
            )
        {
            if (generatedFilePath is null)
            {
                throw new ArgumentNullException(nameof(generatedFilePath));
            }

            try
            {
                File.WriteAllText(generatedFilePath, NewFileBody);
            }
            catch(Exception excp)
            {
                Logging.LogGen($"Writing to '{generatedFilePath}' fails due to:");
                Logging.LogGen(excp);
            }
        }
    }
}
