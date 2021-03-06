using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace DpdtInject.Extension.Helper
{
    public class VisualStudioDocumentHelper
    {
        private readonly string _documentFullPath;
        private readonly IVsUIShellOpenDocument? _openDoc;
        private readonly IVsTextManager? _textManager;

        public VisualStudioDocumentHelper(
            string documentFullPath
            )
        {
            if (documentFullPath is null)
            {
                throw new ArgumentNullException(nameof(documentFullPath));
            }

            _documentFullPath = documentFullPath;
            _openDoc = Package.GetGlobalService(typeof(IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
            _textManager = Package.GetGlobalService(typeof(VsTextManagerClass)) as IVsTextManager;
        }

        public void OpenAndNavigate(
            int startLine,
            int startColumn,
            int endLine,
            int endColumn
            )
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(OpenAndNavigate));

            var logicalView = VSConstants.LOGVIEWID_Code;

            #region trying to determine is document open

            //{
            //    if (ErrorHandler.Failed(
            //        openDoc.IsDocumentInAProject(
            //            documentFullPath,
            //            out var uiHier,
            //            out var itemId0,
            //            out IServiceProvider serviceProvider,
            //            out var docInProj
            //            )
            //        ))
            //    {
            //        return;
            //    }

            //    {
            //        if (ErrorHandler.Failed(
            //            openDoc.IsSpecificDocumentViewOpen(
            //                uiHier,
            //                itemId0,
            //                documentFullPath,
            //                ref logicalView,
            //                "",
            //                (int) __VSIDOFLAGS.IDO_IgnoreLogicalView,
            //                out var hierOpen,
            //                out var itemIdOpen,
            //                out var windowFrame,
            //                out var open
            //                )
            //            ) ||
            //            windowFrame == null)
            //        {
            //            return;
            //        }

            //        //var oleSp = (OLE.Interop.IServiceProvider)Package.GetGlobalService(typeof(OLE.Interop.IServiceProvider));
            //        var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            //        var oleSp = componentModel.GetService<IServiceProvider>();
            //        //var editorAdapterFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            //        var sp = new Microsoft.VisualStudio.Shell.ServiceProvider(oleSp);

            //        var content = VsShellUtilities.GetRunningDocumentContents(
            //            sp,
            //            documentFullPath
            //            );

            //        var r = VsShellUtilities.IsDocumentOpen(
            //            sp,
            //            documentFullPath,
            //            logicalView,
            //            out var hier3,
            //            out var itemId3,
            //            out var windowFrame3
            //            );

            //    }

            //    {
            //        uint[] itemIdOpen = new uint[1];
            //        if (ErrorHandler.Failed(
            //                openDoc.IsDocumentOpen(
            //                    uiHier,
            //                    itemId0,
            //                    documentFullPath,
            //                    ref logicalView,
            //                    (int) __VSIDOFLAGS.IDO_IgnoreLogicalView,
            //                    out var hierOpen,
            //                    itemIdOpen,
            //                    out var windowFrame,
            //                    out var open
            //                    )
            //                ) ||
            //            windowFrame == null)
            //        {
            //            return;
            //        }
            //    }
            //}

            #endregion

            if (!OpenDocument(logicalView, out IVsWindowFrame? frame))
            {
                return;
            }

            frame!.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out object docData);

            if (!GetVsTextBuffer(docData, out VsTextBuffer? buffer))
            {
                return;
            }

            NavigateAndSelect(
                buffer!,
                startLine, 
                startColumn, 
                endLine, 
                endColumn
                );
        }

        private void NavigateAndSelect(
            VsTextBuffer buffer,
            int startLine,
            int startColumn,
            int endLine,
            int endColumn
            )
        {
            if (_textManager == null)
            {
                return;
            }

            var docViewType = default(Guid);

            _textManager.NavigateToLineAndColumn(
                buffer,
                ref docViewType,
                startLine,
                startColumn,
                endLine,
                endColumn
                );
        }

        private static bool GetVsTextBuffer(
            object docData,
            out VsTextBuffer? buffer
            )
        {
            // Get the VsTextBuffer  
            buffer = docData as VsTextBuffer;
            if (buffer == null)
            {
                if (docData is IVsTextBufferProvider bufferProvider)
                {
                    ErrorHandler.ThrowOnFailure(
                        bufferProvider.GetTextBuffer(out IVsTextLines lines)
                        );

                    buffer = lines as VsTextBuffer;

                    Debug.Assert(buffer != null, "IVsTextLines does not implement IVsTextBuffer");

                    if (buffer == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool OpenDocument(
            Guid logicalView,
            out IVsWindowFrame? frame
            )
        {
            if (_openDoc is null)
            {
                frame = null;
                return false;
            }

            if (ErrorHandler.Failed(
                    _openDoc.OpenDocumentViaProject(
                        _documentFullPath,
                        ref logicalView,
                        out IServiceProvider _,
                        out IVsUIHierarchy hier,
                        out var itemId,
                        out frame)
                    ) ||
                frame == null)
            {
                return false;
            }

            return true;
        }
    }
}
