using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Helper;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using DpdtInject.Extension.UI.ViewModel.Details;
using DpdtInject.Extension.Shared.Dto;
using DpdtInject.Extension.UI.ChainStep;
using DpdtInject.Generator.Core.Binding.Xml;

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

            var codelensTarget = tag.Target as CodeLensTarget;

            var window = new AddBindingWindow(
                async anw =>
                {
                    var choosedParameters = new ChoosedParameters(codelensTarget);

                    var apcs = new AdditionalParametersChainStep(
                        anw,
                        anw.CenterContentControl,
                        choosedParameters
                        );

                    var tmcs = new TargetMethodsChainStep(
                        anw.CenterContentControl,
                        choosedParameters
                        );

                    var bfcs = new BindsFromChainStep(
                        anw.CenterContentControl,
                        choosedParameters
                        );

                    var cacs = new ConstructorArgumentsChainStep(
                        anw.CenterContentControl,
                        choosedParameters
                        );

                    var clcs = new ConstructorListChainStep(
                        anw.CenterContentControl,
                        choosedParameters
                        );

                    apcs.SetSteps(tmcs);
                    tmcs.SetSteps(bfcs, apcs);
                    bfcs.SetSteps(cacs, clcs, tmcs);
                    cacs.SetSteps(clcs, bfcs);
                    clcs.SetSteps(cacs, bfcs);

                    await clcs.CreateAsync();
                }
                );


            window.ShowModal();
        }

        private void Unbind_OnMouseLeftButtonUp(
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

            if (!TryGetBinding(tag.BindingIdentifier, out var containerAndScanner, out var binding))
            {
                return;
            }

            if (binding!.IsConventional)
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

            //switch to modified document
            var modifiedDocumentHelper = new VisualStudioDocumentHelper(
                binding!.Position.FilePath
                );

            modifiedDocumentHelper.OpenAndNavigate(
                binding!.Position.StartLine,
                binding!.Position.StartColumn,
                binding!.Position.EndLine,
                binding!.Position.EndColumn
                );

            try
            {
                dte.UndoContext.Open($"Remove bind {binding.TargetRepresentation}");

                ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out var activeView));

                var editorAdapter = componentModel.GetService<IVsEditorAdaptersFactoryService>();
                var textView = editorAdapter.GetWpfTextView(activeView);
                
                if (textView != null)
                {
                    textView.TextBuffer.Delete(
                        new Span(
                            binding.Position.SpanStart,
                            binding.Position.SpanLength
                            )
                        );
                }
            }
            finally
            {
                dte.UndoContext.Close();
            }

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
        }

        private void GoTo_OnMouseLeftButtonUp(
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

            if (!TryGetBinding(bindingIdentifier, out _, out var binding))
            {
                return;
            }

            var documentHelper = new VisualStudioDocumentHelper(
                binding!.Position.FilePath
                );

            documentHelper.OpenAndNavigate(
                binding!.Position.StartLine,
                binding!.Position.StartColumn,
                binding!.Position.EndLine,
                binding!.Position.EndColumn
                );

        }

        private static bool TryGetBinding(
            Guid bindingIdentifier,
            out ContainerAndScanner? containerAndScanner,
            out IBindingStatement? binding
            )
        {
            var componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
            if (componentModel == null)
            {
                containerAndScanner = null;
                binding = null;
                return false;
            }

            var bmc = componentModel.GetService<ContainerAndScanner>();
            var solutionBinds = bmc.Binds;
            if (solutionBinds == null)
            {
                containerAndScanner = null;
                binding = null;
                return false;
            }

            if (!solutionBinds.TryGetBinding(bindingIdentifier, out binding))
            {
                containerAndScanner = null;
                binding = null;
                return false;
            }

            containerAndScanner = bmc;
            return true;
        }

    }
}
