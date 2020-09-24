using DpdtInject.Generator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using System;
using System.IO;

namespace DpdtInject.Generator
{
    public class ModificationDescription
    {
        public INamedTypeSymbol ModifiedType { get; }

        public string ModifiedTypeName => ModifiedType.Name;

        public string ModifiedTypeFullName => ModifiedType.GetFullName();

        public string NewFileName { get; }

        public string NewFileBody { get; }

        public ModificationDescription(
            INamedTypeSymbol modifiedType,
            string newFileName,
            string newFileBody,
            bool needToNormalizeWhitespaces
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

            //make this generated code beautify a bit
            if(needToNormalizeWhitespaces)
            {
                NewFileBody = SyntaxFactory.ParseCompilationUnit(newFileBody).NormalizeWhitespace().GetText().ToString();
            }
            else
            {
                NewFileBody = newFileBody;
            }
        }

    }
}
