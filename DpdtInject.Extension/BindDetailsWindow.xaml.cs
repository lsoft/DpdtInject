using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.ViewModel.Details;
using DpdtInject.Generator.Binding;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DpdtInject.Extension
{
    /// <summary>
    /// Interaction logic for BindDetailsWindow.xaml
    /// </summary>
    public partial class BindDetailsWindow : UserControl
    {
        public BindDetailsWindow()
        {
            InitializeComponent();
        }

        private void BindDetailsWindow_OnLoaded(
            object sender,
            RoutedEventArgs e
            )
        {
            UpdateBinding();
        }


        private void UpdateBinding()
        {
            var listViews = this.FindLogicalChildren<ListView>().ToList();

            foreach (var listView in listViews)
            {
                listView.UpdateLayout();

                if (listView.View is GridView gridView)
                {
                    foreach (var column in gridView.Columns)
                    {
                        var binding = BindingOperations.GetBindingExpressionBase(
                            column,
                            GridViewColumn.WidthProperty
                            );

                        binding?.UpdateTarget();
                    }
                }
            }
        }


        private void CreateBinding_OnMouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e
            )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var tag = (sender as FrameworkElement)?.Tag as DpdtBindingReferenceSetViewModel;
            if (tag is null)
            {
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2;
            if (dte == null)
            {
                return;
            }

            var window = new AddBindingWindow(
                tag.Target
                );
            //window.Owner = dte.MainWindow;

            window.ShowModal();
        }

        private void BindUnbind_OnMouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e
            )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var tag = (sender as FrameworkElement)?.Tag as DpdtBindingTargetViewModel;
            if (tag is null)
            {
                return;
            }

            if (!TryGetBindingContainer(tag.BindingIdentifier, out var containerAndScanner, out var bindingContainer))
            {
                return;
            }

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

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out var currentActiveView));

            var currentDocumentFilePath = System.IO.Path.Combine(dte.ActiveDocument.Path, dte.ActiveDocument.Name);
            currentActiveView.GetCaretPos(out var currentLine, out var currentColumn);

            //var textDocumentFactoryService = Package.GetGlobalService(typeof(ITextDocumentFactoryService)) as ITextDocumentFactoryService;

            //currentActiveView..GetBuffer(out var buffer);

            //var currentDoc = textDocumentFactoryService.TryGetTextDocument(
            //    buffer.text,
            //    out var textDocument
            //    );


            //switch to modified document
            var expressionNode = bindingContainer!.ExpressionNode;
            var location = expressionNode.GetLocation();
            var lineSpan = location.GetLineSpan();

            var modifiedDocumentHelper = new VisualStudioDocumentHelper(
                expressionNode.SyntaxTree.FilePath
                );

            modifiedDocumentHelper.OpenAndNavigate(
                lineSpan.StartLinePosition.Line,
                lineSpan.StartLinePosition.Character,
                lineSpan.EndLinePosition.Line,
                lineSpan.EndLinePosition.Character
                );


            //var workspace = (Workspace)componentModel.GetService<VisualStudioWorkspace>();
            //if (workspace == null)
            //{
            //    return;
            //}

            //var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(
            //    expressionNode.SyntaxTree.FilePath
            //    );
            //if (documentId.Length != 1)
            //{
            //    return;
            //}

            //var document =  workspace.CurrentSolution.GetDocument(documentId[0]);
            //if (document == null)
            //{
            //    return;
            //}

            try
            {
                dte.UndoContext.Open($"Remove bind {bindingContainer.TargetRepresentation}");

                ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out var activeView));

                var editorAdapter = componentModel.GetService<IVsEditorAdaptersFactoryService>();
                var textView = editorAdapter.GetWpfTextView(activeView);
                
                if (textView != null)
                {
                    textView.TextBuffer.Delete(
                        new Span(
                            bindingContainer.ExpressionNode.Span.Start,
                            bindingContainer.ExpressionNode.Span.Length
                            )
                        );

                    //document = document!.WithText(SourceText.From("//123"));
                }
            }
            finally
            {
                dte.UndoContext.Close();
            }

            //workspace.TryApplyChanges(document.Project.Solution);


            //get back to our document
            var sourceDocumentHelper = new VisualStudioDocumentHelper(
                currentDocumentFilePath
                );

            sourceDocumentHelper.OpenAndNavigate(
                currentLine,
                currentColumn,
                currentLine,
                currentColumn
                );

            //System.Threading.Tasks.Task.Run(
            //    () => containerAndScanner!.AsyncStartScan()
            //    ).FileAndForget(nameof(BindUnbind_OnMouseLeftButtonUp));
        }

        private void GoToMethod_OnMouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e
            )
        {
            var tag = (sender as FrameworkElement)?.Tag as DpdtBindingTargetViewModel;
            if (tag is null)
            {
                return;
            }

            NavigateTo(tag.BindingIdentifier);
        }
        
        public void NavigateTo(
            Guid bindingIdentifier
            )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!TryGetBindingContainer(bindingIdentifier, out _, out var bindingContainer))
            {
                return;
            }

            var expressionNode = bindingContainer!.ExpressionNode;
            var location = expressionNode.GetLocation();
            var lineSpan = location.GetLineSpan();

            var documentHelper = new VisualStudioDocumentHelper(
                expressionNode.SyntaxTree.FilePath
                );

            documentHelper.OpenAndNavigate(
                lineSpan.StartLinePosition.Line,
                lineSpan.StartLinePosition.Character,
                lineSpan.EndLinePosition.Line,
                lineSpan.EndLinePosition.Character
                );

        }

        private static bool TryGetBindingContainer(
            Guid bindingIdentifier,
            out ContainerAndScanner? containerAndScanner,
            out IBindingContainer? bindingContainer
            )
        {
            var componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
            if (componentModel == null)
            {
                containerAndScanner = null;
                bindingContainer = null;
                return false;
            }

            var bmc = componentModel.GetService<ContainerAndScanner>();
            var solutionBinds = bmc.Binds;
            if (solutionBinds == null)
            {
                containerAndScanner = null;
                bindingContainer = null;
                return false;
            }

            if (!solutionBinds.TryGetBindingContainer(bindingIdentifier, out bindingContainer))
            {
                containerAndScanner = null;
                bindingContainer = null;
                return false;
            }

            containerAndScanner = bmc;
            return true;
        }

    }
}
