using System;
using DpdtInject.Extension.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using EnvDTE80;
using Task = System.Threading.Tasks.Task;
using DpdtInject.Generator.Core.Binding.Xml;

namespace DpdtInject.Extension.Machinery.Add
{
    public class DocumentModifier
    {
        private readonly IMethodBindContainer _targetMethod;

        public DocumentModifier(
            IMethodBindContainer targetMethod
            )
        {
            if (targetMethod is null)
            {
                throw new ArgumentNullException(nameof(targetMethod));
            }

            _targetMethod = targetMethod;
        }

        public async Task DoSurgeryAsync(
            NewBindingInfo newBindingInfo
            )
        {
            if (newBindingInfo is null)
            {
                throw new ArgumentNullException(nameof(newBindingInfo));
            }

            if (string.IsNullOrEmpty(_targetMethod.MethodDeclaration.Position.FilePath))
            {
                return;
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2;
            if (dte == null)
            {
                return;
            }

            var componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
            if (componentModel == null)
            {
                return;
            }

            var textManager = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager;
            if (textManager == null)
            {
                return;
            }

            var workspace = (Workspace)componentModel.GetService<VisualStudioWorkspace>();
            if (workspace == null)
            {
                return;
            }

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out var currentActiveView));

            var currentDocumentFilePath = System.IO.Path.Combine(dte.ActiveDocument.Path, dte.ActiveDocument.Name);
            currentActiveView.GetCaretPos(out var currentLine, out var currentColumn);

            #region open modified document

            var modifiedDocumentHelper = new VisualStudioDocumentHelper(
                _targetMethod.MethodDeclaration.Position.FilePath
                );

            modifiedDocumentHelper.OpenAndNavigate(
                _targetMethod.MethodDeclaration.Position.StartLine,
                _targetMethod.MethodDeclaration.Position.StartColumn,
                _targetMethod.MethodDeclaration.Position.EndLine,
                _targetMethod.MethodDeclaration.Position.EndColumn
                );

            #endregion

            #region switch back to source document if needed

            if (newBindingInfo.IsBindingComplete)
            {
                var sourceDocumentHelper = new VisualStudioDocumentHelper(
                    currentDocumentFilePath
                    );

                sourceDocumentHelper.OpenAndNavigate(
                    currentLine,
                    currentColumn,
                    currentLine,
                    currentColumn
                    );
            }

            #endregion

            var document = workspace.GetDocument(
                _targetMethod.MethodDeclaration.Position.FilePath
                );
            if (document == null)
            {
                return;
            }

            var surgeon = new SyntaxSurgeon(
                _targetMethod
                );

            var (surgedDocument, addedBinding) = await surgeon.SurgeAsync(
                document,
                newBindingInfo
                );

            if (surgedDocument == null)
            {
                return;
            }

            if (!workspace.TryApplyChanges(surgedDocument.Project.Solution))
            {
                DpdtPackage.ShowMessageBox(
                    "Error",
                    "Error happened while additing a new binding clause. Please try again."
                    );
                return;
            }

            if (!newBindingInfo.IsBindingComplete && addedBinding != null)
            {
                var addedBindingLineSpan = addedBinding.GetLocation().GetLineSpan();
                modifiedDocumentHelper.OpenAndNavigate(
                    addedBindingLineSpan.StartLinePosition.Line,
                    addedBindingLineSpan.StartLinePosition.Character,
                    addedBindingLineSpan.EndLinePosition.Line,
                    addedBindingLineSpan.EndLinePosition.Character
                    );
            }
        }
    }
}
