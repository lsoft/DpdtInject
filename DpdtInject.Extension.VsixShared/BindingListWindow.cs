using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using DpdtInject.Extension.UI.Control.BindingList;
using DpdtInject.Extension.UI.ViewModel.BindingList;

namespace DpdtInject.Extension
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("a9e3e7e8-508b-4f36-bd1f-b4a495de0fed")]
    public class BindingListWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListWindow"/> class.
        /// </summary>
        public BindingListWindow() : base(null)
        {
            this.Caption = "Dpdt Binding List";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            var control = new BindingListWindowControl();
            control.DataContext = new BindingListViewModel();

            this.Content = control;
        }
    }
}
