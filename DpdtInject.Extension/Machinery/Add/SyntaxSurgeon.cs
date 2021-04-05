using System;
using System.Linq;
using System.Threading.Tasks;
using DpdtInject.Extension.Container;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using DpdtInject.Extension.Options;
using DpdtInject.Extension.Machinery.Add;
using DpdtInject.Generator.Core.Binding.Xml;

namespace DpdtInject.Extension.Machinery.Add
{
    public class SyntaxSurgeon
    {
        private readonly IMethodBindContainer _targetMethod;

        public SyntaxSurgeon(
            IMethodBindContainer targetMethod
            )
        {
            if (targetMethod is null)
            {
                throw new ArgumentNullException(nameof(targetMethod));
            }

            _targetMethod = targetMethod;
        }

        public async Task<(Document?, SyntaxNode?)> SurgeAsync(
            Document document,
            NewBindingInfo newBindingInfo
            )
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (newBindingInfo is null)
            {
                throw new ArgumentNullException(nameof(newBindingInfo));
            }

            var documentEditor = await DocumentEditor.CreateAsync(
                document
                );

            #region add new namespaces

            var existingUsings = documentEditor.OriginalRoot
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .ToList()
                ;

            var additionalNamespaces = newBindingInfo.GetUniqueUsings(existingUsings).ToList();
            
            if (existingUsings.Count > 0)
            {
                documentEditor.InsertAfter(existingUsings.Last(), additionalNamespaces);
            }
            else
            {
                documentEditor.InsertAfter(
                    documentEditor.OriginalRoot.DescendantNodes().First(),
                    additionalNamespaces
                    );
            }

            #endregion

            #region add new binding

            var methodSyntax = documentEditor.OriginalRoot
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(m => m.Identifier.Text == _targetMethod.MethodDeclaration.MethodName)
                ;
            if (methodSyntax == null)
            {
                return (null, null);
            }
            if (methodSyntax.Body == null)
            {
                return (null, null);
            }

            var leadingTrivia = methodSyntax.Body.GetLeadingTrivia().ToString();


            var syntaxAnnotation = new SyntaxAnnotation();
            var bcp = new BindClauseProducer(newBindingInfo);
            var producedBinding = bcp.ProduceBinding(leadingTrivia)
                .WithAdditionalAnnotations(syntaxAnnotation);

            var modifiedMethodSyntax = methodSyntax.WithBody(
                methodSyntax.Body.AddStatements(new[] { producedBinding })
                );

            documentEditor.ReplaceNode(
                methodSyntax,
                modifiedMethodSyntax
                );

            #endregion

            var changedDocument = documentEditor.GetChangedDocument();
            var changedRoot = await changedDocument.GetSyntaxRootAsync();

            var opts = GeneralOptions.Instance;
            if (changedRoot != null && opts.EnableWhitespaceNormalization)
            {
                var changedSyntaxRoot = changedRoot.NormalizeWhitespace();
                changedDocument = changedDocument.WithSyntaxRoot(changedSyntaxRoot);
                changedRoot = await changedDocument.GetSyntaxRootAsync();
            }

            var addedBinding = changedRoot
                ?.DescendantNodes()
                .Where(n => n.HasAnnotation(syntaxAnnotation))
                .FirstOrDefault()
                ?? null;

            return (changedDocument, addedBinding);
        }
    }
}
