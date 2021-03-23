using System;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using EnvDTE80;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension.AddMachinery
{
    public class DocumentModifier
    {
        private readonly MethodBindContainer _targetMethod;

        public DocumentModifier(
            MethodBindContainer targetMethod
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


            var methods = _targetMethod.ClusterType.GetMembers(_targetMethod.MethodSyntax.Identifier.Text);
            if (methods.Length != 1)
            {
                return;
            }

            var method = methods[0];
            if (method.Locations.Length != 1)
            {
                return;
            }

            var methodLocation = method.Locations[0];
            var methodFilePath = methodLocation.SourceTree?.FilePath;

            if (methodFilePath == null)
            {
                return;
            }

            #region open modified document

            var modifiedDocumentHelper = new VisualStudioDocumentHelper(
                methodFilePath
                );

            var lineSpan = methodLocation.GetLineSpan();
            modifiedDocumentHelper.OpenAndNavigate(
                lineSpan.StartLinePosition.Line,
                lineSpan.StartLinePosition.Character,
                lineSpan.EndLinePosition.Line,
                lineSpan.EndLinePosition.Character
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
                methodFilePath
                );
            if (document == null)
            {
                return;
            }

            //OptionSet options = workspace.Options;
            //options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, true);
            //options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInProperties, true);


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
